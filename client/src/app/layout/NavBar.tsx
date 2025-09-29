import { AppBar, Box, Button, Container, MenuItem, Toolbar, Typography } from '@mui/material';
import { Group } from "@mui/icons-material";

type Props = {
    openForm: () => void,
};

export default function NavBar({ openForm }: Props) {
    return (
        <Box sx={{ flexGrow: 1 }}>
            <AppBar position="static" sx={{
                backgroundImage: 'linear-gradient(135deg, #182A73 0%, #218AAE 69%, #20A7AC 89%)'
            }}>
                <Container maxWidth="xl">
                    <Toolbar sx={{ display: "flex", justifyContent: "space-between" }}>
                        <Box>
                            <MenuItem sx={{ display: "flex", gap: 2 }}>
                                <Group fontSize="large" />
                                <Typography variant="h4" fontWeight="bold">Reactivities</Typography>
                            </MenuItem>
                        </Box>
                        <Box sx={{ display: "flex" }}>
                            <MenuItem sx={{
                                fontSIze: "1.2rem",
                                textTransform: "uppercase",
                                fontWeight: "bold"
                            }}>
                                Activities
                            </MenuItem>
                            <MenuItem sx={{
                                fontSIze: "1.2rem",
                                textTransform: "uppercase",
                                fontWeight: "bold"
                            }}>
                                About
                            </MenuItem>
                            <MenuItem sx={{
                                fontSIze: "1.2rem",
                                textTransform: "uppercase",
                                fontWeight: "bold"
                            }}>
                                Contact
                            </MenuItem>
                        </Box>
                        <Button
                            size="large"
                            variant="contained"
                            color="warning"
                            onClick={openForm}
                        >
                            Create activity
                        </Button>
                    </Toolbar>
                </Container>
            </AppBar>
        </Box>
    );
}