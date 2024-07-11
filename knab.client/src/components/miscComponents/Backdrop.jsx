import { Backdrop, CircularProgress } from "@mui/material";
import { useSelector } from "react-redux";

export default function BackdropComponent() {
  const show = useSelector((s) => s.display.showLoader);

  return (
    <Backdrop open={show}>
      <CircularProgress style={{ color: "blue" }} />
    </Backdrop>
  );
}
