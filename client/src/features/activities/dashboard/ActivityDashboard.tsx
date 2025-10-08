import { Grid } from '@mui/material';
import ActivityList from './ActivityList';
import ActivityDetails from '../details/ActivityDetails';
import ActivityForm from '../form/ActivityForm';

type Props = {
    activities: Activity[],
    selectActivity: (id: string) => void,
    cancelSelectActivity: () => void,
    activity: Activity,
    editMode: boolean,
    openForm: (id: string) => void,
    closeForm: () => void
}

export default function ActivityDashboard(props: Readonly<Props>) {
    const { activities,
        selectActivity,
        cancelSelectActivity,
        activity: activity,
        editMode,
        openForm,
        closeForm
    } = props;

    return (
        <Grid container spacing={3}>
            <Grid size={7}>
                <ActivityList
                    activities={activities}
                    selectActivity={selectActivity}
                />
            </Grid>
            <Grid size={5}>
                {
                    activity && !editMode &&
                    <ActivityDetails
                        selectedActivity={activity}
                        cancelSelectActivity={cancelSelectActivity}
                        openForm={openForm}
                    />
                }
                {
                    editMode && <ActivityForm closeForm={closeForm} activity={activity} />
                }
            </Grid>
        </Grid >
    )
}
