import { Grid } from "@mui/material";
import ActivityList from "./ActivityList";
import ActivityFilter from "./ActivityFilter";

export default function ActivityDashboard() {
    return (
        <Grid container spacing={3}>
            <Grid size={8}>
                <ActivityList />
            </Grid>
            <Grid
                size={4}
                sx={{ position: "sticky", top: 70, alignSelf: "flex-start" }}
            >
                <ActivityFilter />
            </Grid>
        </Grid>
    );
}
