import { useParams } from "react-router";
import { useProfile } from "../../lib/hooks/useProfile";
import {
    Box,
    Button,
    Divider,
    ImageList,
    ImageListItem,
    Typography,
} from "@mui/material";
import { useState } from "react";
import PhotoUploadWidget from "../../app/shared/components/PhotoUploadWidget";
import StarButton from "../../app/shared/components/StarButton";
import DeleteButton from "../../app/shared/components/DeleteButton";

export default function ProfilePhotos() {
    const { id } = useParams();
    const {
        photos,
        isLoadingPhotos,
        isCurrentUser,
        uploadPhoto,
        profile,
        setMainPhoto,
        deletePhoto,
    } = useProfile(id);
    const [editMode, setEditMode] = useState(false);

    const handlePhotoUpload = (file: Blob) => {
        uploadPhoto.mutate(file, {
            onSuccess: () => {
                setEditMode(false);
            },
        });
    };

    if (isLoadingPhotos) return <Typography>Loading photos...</Typography>;

    if (!photos) return <Typography>No photos not found</Typography>;

    const uploadPathToBeReplaced = "/upload/";
    const uploadPathReplacedWith = "/upload/w_160,h_160,c_fill,f_auto,g_face";

    return (
        <Box>
            <Box display={"flex"} justifyContent="space-between">
                <Typography variant="h5" gutterBottom>
                    Photos
                </Typography>
                {isCurrentUser && (
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={() => setEditMode(!editMode)}
                    >
                        {editMode ? "Cancel" : "Add Photos"}
                    </Button>
                )}
            </Box>
            <Divider sx={{ my: 2 }} />
            {editMode ? (
                <PhotoUploadWidget
                    uploadPhoto={handlePhotoUpload}
                    loading={uploadPhoto.isPending}
                />
            ) : (
                <>
                    {photos.length === 0 ? (
                        <Typography>No photos available</Typography>
                    ) : (
                        <ImageList
                            sx={{
                                height: 450,
                                border: 1,
                                borderColor: "grey.300",
                            }}
                            cols={6}
                            rowHeight={164}
                        >
                            {photos.map((item) => {
                                return (
                                    <ImageListItem
                                        key={item.id}
                                        sx={{
                                            border: 1,
                                            borderColor: "grey.300",
                                            m: 0.5,
                                        }}
                                    >
                                        <img
                                            srcSet={`${item.url.replace(
                                                uploadPathToBeReplaced,
                                                uploadPathReplacedWith +
                                                    ",dpr_2/",
                                            )}`}
                                            src={`${item.url.replace(
                                                uploadPathToBeReplaced,
                                                uploadPathReplacedWith + "/",
                                            )}`}
                                            alt={"Profile Photo"}
                                            loading="lazy"
                                        />
                                        {isCurrentUser && (
                                            <div>
                                                <Box
                                                    sx={{
                                                        position: "absolute",
                                                        top: 0,
                                                        left: 0,
                                                    }}
                                                    onClick={() =>
                                                        setMainPhoto.mutate(
                                                            item,
                                                        )
                                                    }
                                                >
                                                    <StarButton
                                                        selected={
                                                            item.url ===
                                                            profile?.imageUrl
                                                        }
                                                    />
                                                </Box>
                                                {item.url !==
                                                    profile?.imageUrl && (
                                                    <Box
                                                        sx={{
                                                            position:
                                                                "absolute",
                                                            top: 0,
                                                            right: 0,
                                                        }}
                                                        onClick={() =>
                                                            deletePhoto.mutate(
                                                                item.id,
                                                            )
                                                        }
                                                    >
                                                        <DeleteButton />
                                                    </Box>
                                                )}
                                            </div>
                                        )}
                                    </ImageListItem>
                                );
                            })}
                        </ImageList>
                    )}
                </>
            )}
        </Box>
    );
}
