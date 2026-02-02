import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";
import { useMemo } from "react";

export const useProfile = (id?: string) => {
    const queryClient = useQueryClient();

    const { data: profile, isLoading: isLoadingProfile } = useQuery<Profile>({
        queryKey: ["profile", id],
        queryFn: async () => {
            const response = await agent.get<Profile>(`/profiles/${id}`);
            return response.data;
        },
        enabled: !!id,
    });

    const { data: photos, isLoading: isLoadingPhotos } = useQuery<Photo[]>({
        queryKey: ["photos", id],
        queryFn: async () => {
            const response = await agent.get<Photo[]>(`/profiles/${id}/photos`);
            return response.data;
        },
        enabled: !!id,
    });

    const uploadPhoto = useMutation({
        mutationFn: async (file: Blob) => {
            const formData = new FormData();
            formData.append("file", file);
            const response = await agent.post<Photo>(
                `/profiles/add-photo`,
                formData,
                {
                    headers: { "Content-Type": "multipart/form-data" },
                },
            );
            return response.data;
        },
        onSuccess: (photo) => {
            queryClient.invalidateQueries({ queryKey: ["photos", id] });
            queryClient.setQueryData(["user"], (data: User) => {
                if (!data) return data;
                return {
                    ...data,
                    imageUrl: data.imageUrl ?? photo.url,
                };
            });
            queryClient.setQueryData(["profile", id], (data: Profile) => {
                if (!data) return data;
                return {
                    ...data,
                    imageUrl: data.imageUrl ?? photo.url,
                };
            });
        },
    });

    const setMainPhoto = useMutation({
        mutationFn: async (photo: Photo) => {
            await agent.put(`/profiles/${photo.id}/setMain`);
        },
        onSuccess: (_, photo) => {
            queryClient.setQueryData(["user"], (data: User) => {
                if (!data) return data;
                return {
                    ...data,
                    imageUrl: photo.url,
                };
            });
            queryClient.setQueryData(["profile", id], (data: Profile) => {
                if (!data) return data;
                return {
                    ...data,
                    imageUrl: photo.url,
                };
            });
        },
    });

    const deletePhoto = useMutation({
        mutationFn: async (photoId: string) => {
            await agent.delete(`/profiles/${photoId}/photos`);
        },
        onSuccess: (_, photoId) => {
            queryClient.setQueryData(["photos", id], (photos: Photo[]) => {
                return photos?.filter((p) => p.id !== photoId);
            });
        },
    });

    const isCurrentUser = useMemo(() => {
        const newLocal = queryClient.getQueryData<User>(["user"])?.id;
        return id === newLocal;
    }, [id, queryClient]);

    return {
        profile,
        isLoadingProfile,
        photos,
        isLoadingPhotos,
        isCurrentUser,
        uploadPhoto,
        setMainPhoto,
        deletePhoto,
    };
};
