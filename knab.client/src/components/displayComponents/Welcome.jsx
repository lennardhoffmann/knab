import { Button } from "@mui/material";
import { useDispatch } from "react-redux";
import { setView } from "../../state/displaySlice";
import "./_style.displayComponents.Shared.scss";

export default function WelcomeComponent() {
  const dispatch = useDispatch();

  return (
    <div className="containerStyling">
      <label className="labelStyling">Welcome to my Knab assessment</label>
      <label className="labelStyling">
        To start using the UI click the button below
      </label>
      <Button
        color="primary"
        variant="contained"
        style={{ width: "30%" }}
        onClick={() => dispatch(setView("Login"))}
      >
        {`Let's get started`}
      </Button>
    </div>
  );
}
