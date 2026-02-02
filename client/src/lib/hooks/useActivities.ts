import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { useLocation } from "react-router";
import { useAccount } from "./useAccount";

export const useActivities = (id?: string) => {
    const queryClient = useQueryClient();
    const location = useLocation();
    const { currentUser } = useAccount();
    const activitiesKey = "activities";
    const activitiesQueryKey = [activitiesKey];

    const { data: activities, isLoading } = useQuery({
        queryKey: activitiesQueryKey,
        queryFn: async () => {
            const response = await agent.get<Activity[]>(`/${activitiesKey}`);
            return response.data;
        },
        // staleTime: 1000 * 60 * 5 //5 Minutes
        enabled:
            !id && location.pathname === `/${activitiesKey}` && !!currentUser,
        select: (data) => data.map((activity) => getActivity(activity)),
    });

    const getActivity = (data: Activity): Activity => {
        const host = data.attendees.find((x) => x.id === data.hostId);
        return {
            ...data,
            isHost: currentUser?.id === data.hostId,
            isGoing: data.attendees.some((x) => x.id === currentUser?.id),
            hostImageUrl: host?.imageUrl,
        };
    };

    const { data: activity, isLoading: isLoadingActivity } = useQuery({
        queryKey: [activitiesKey, id],
        queryFn: async () => {
            const response = await agent.get<Activity>(
                `/${activitiesKey}/${id}`,
            );
            return response.data;
        },
        enabled: !!id && !!currentUser,
        select: (data) => getActivity(data),
    });

    const updateActivity = useMutation({
        mutationFn: async (activity: Activity) => {
            await agent.put(`/${activitiesKey}`, activity);
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: activitiesQueryKey,
            });
        },
    });

    const createActivity = useMutation({
        mutationFn: async (activity: Activity) => {
            const response = await agent.post(`/${activitiesKey}`, activity);
            return response.data;
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: activitiesQueryKey,
            });
        },
    });

    const deleteActivity = useMutation({
        mutationFn: async (id: string) => {
            await agent.delete(`/${activitiesKey}/${id}`);
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: activitiesQueryKey,
            });
        },
    });

    const updateAttendance = useMutation({
        mutationFn: async (id: string) => {
            await agent.post(`/${activitiesKey}/${id}/attend`);
        },
        onMutate: async (activityId: string) => {
            await queryClient.cancelQueries({
                queryKey: [activitiesKey, activityId],
            });

            const previousActivity = queryClient.getQueryData<Activity>([
                activitiesKey,
                activityId,
            ]);

            queryClient.setQueryData<Activity>(
                [activitiesKey, activityId],
                (oldActivity) => {
                    if (!oldActivity || !currentUser) {
                        return oldActivity;
                    }

                    const isHost = oldActivity.hostId === currentUser.id;
                    const isAttending = oldActivity.attendees.some(
                        (x) => x.id === currentUser.id,
                    );

                    return {
                        ...oldActivity,
                        isCancelled: isHost
                            ? !oldActivity.isCancelled
                            : oldActivity.isCancelled,
                        attendees: isAttending
                            ? isHost
                                ? oldActivity.attendees
                                : oldActivity.attendees.filter(
                                      (x) => x.id !== currentUser.id,
                                  )
                            : [
                                  ...oldActivity.attendees,
                                  {
                                      id: currentUser.id,
                                      displayName: currentUser.displayName,
                                      imageUrl: currentUser.imageUrl,
                                  },
                              ],
                    };
                },
            );

            return { previousActivity };
        },
        onError: (error, activityId, context) => {
            console.log(context);
            console.log(error);
            if (context?.previousActivity) {
                queryClient.setQueryData<Activity>(
                    [activitiesKey, activityId],
                    context.previousActivity,
                );
            }
        },
    });

    return {
        activitiesKey,
        activities,
        isLoading,
        updateActivity,
        createActivity,
        deleteActivity,
        activity,
        isLoadingActivity,
        updateAttendance,
    };
};
