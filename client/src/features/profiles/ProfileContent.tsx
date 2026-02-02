import { Box, Paper, Tab, Tabs } from "@mui/material";
import { useState, type SyntheticEvent } from "react";
import ProfilePhotos from "./ProfilePhotos";
import ProfileAbout from "./ProfileAbout";

export default function ProfileContent() {
    const [value, setValue] = useState(0);

    const handleChange = (_: SyntheticEvent, newValue: number) => {
        setValue(newValue);
    };

    const tabContents = [
        { label: "About", content: <ProfileAbout /> },
        { label: "Photos", content: <ProfilePhotos /> },
        { label: "Events", content: <div>Events</div> },
        { label: "Followers", content: <div>Followers</div> },
        { label: "Following", content: <div>Following</div> },
    ];

    return (
        <Box
            component={Paper}
            mt={2}
            p={3}
            elevation={3}
            height={530}
            sx={{ display: "flex", alignItems: "flex-start", borderRadius: 3 }}
        >
            <Tabs
                orientation="vertical"
                value={value}
                onChange={handleChange}
                sx={{ borderRight: 1, height: 450, minWidth: 200 }}
            >
                {tabContents.map((tab, index) => {
                    return <Tab label={tab.label} key={index} sx={{ mr: 3 }} />;
                })}
            </Tabs>
            <Box sx={{ flexGrow: 1, p: 3, paddingTop: 0 }}>
                {tabContents[value].content}
            </Box>
        </Box>
    );
}
