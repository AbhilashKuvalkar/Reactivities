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
    closeForm: () => void,
    submitForm: (activity: Activity) => void,
    deleteActivity: (id: string) => void
}

export default function ActivityDashboard(props: Props) {
    const { activities,
        selectActivity,
        cancelSelectActivity,
        activity: activity,
        editMode,
        openForm,
        closeForm,
        submitForm,
        deleteActivity
    } = props;

    return (
        <Grid container spacing={3}>
            <Grid size={7}>
                <ActivityList
                    activities={activities}
                    selectActivity={selectActivity}
                    deleteActivity={deleteActivity}
                />
            </Grid>
            <Grid size={5}>
                {
                    activity && !editMode &&
                    <ActivityDetails
                        activity={activity}
                        cancelSelectActivity={cancelSelectActivity}
                        openForm={openForm}
                    />
                }
                {
                    editMode && <ActivityForm closeForm={closeForm} activity={activity} submitForm={submitForm} />
                }
            </Grid>
        </Grid >
    )
}
