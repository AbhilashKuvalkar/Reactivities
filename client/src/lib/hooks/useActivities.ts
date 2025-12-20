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
    enabled: !id && location.pathname === `/${activitiesKey}` && !!currentUser
  });

  const { data: activity, isLoading: isLoadingActivity } = useQuery({
    queryKey: [activitiesKey, id],
    queryFn: async () => {
      const response = await agent.get<Activity>(`/${activitiesKey}/${id}`);
      return response.data;
    },
    enabled: !!id && !!currentUser,
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

  return {
    activitiesKey,
    activities,
    isLoading,
    updateActivity,
    createActivity,
    deleteActivity,
    activity,
    isLoadingActivity,
  };
};
