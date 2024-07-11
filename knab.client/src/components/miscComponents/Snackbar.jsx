import { useState } from "react";
import Snackbar from "@mui/material/Snackbar";
import { useSelector } from "react-redux";
import { setSnackbar } from "../../state/displaySlice";
import store from "../../store";

export default function SnackbarComponent() {
  const state = useSelector((s) => s.display.showSnackbar);
  const [anchor, setAnchor] = useState({
    vertical: "bottom",
    horizontal: "center",
  });

  if (state.show) {
    setTimeout(() => {
      store.dispatch(setSnackbar({ show: false, message: "" }));
    }, 4000);
  }

  const { vertical, horizontal } = anchor;

  return (
    <Snackbar
      open={state.show}
      message={state.message}
      anchorOrigin={{ vertical, horizontal }}
    />
  );
}
