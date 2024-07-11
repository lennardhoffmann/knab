import { useState } from "react";
import { Button, TextField } from "@mui/material";
import { AuthService } from "../../services";
import { useDispatch } from "react-redux";
import { setLoader, setView } from "../../state/displaySlice";
import "./_style.displayComponents.Shared.scss";

export default function LoginComponent() {
  const [state, setState] = useState({
    UserName: null,
    Password: null,
  });
  const dispatch = useDispatch();

  const handleLogin = () => {
    dispatch(setLoader(true));
    AuthService.Login(state).then(() => {
      dispatch(setView("Data"));

      setTimeout(() => {
        dispatch(setLoader(false));
      }, 1500);
    });
  };

  const { UserName, Password } = state;
  return (
    <div className="containerStyling">
      <label className="labelStyling">
        Please provide to credentials as specified in the ReadMe file of the
        main solution
      </label>
      <TextField
        variant="outlined"
        label="UserName"
        style={{ marginBottom: "20px" }}
        value={UserName || ""}
        onChange={(e) => setState({ ...state, UserName: e.target.value })}
      />
      <TextField
        variant="outlined"
        label="Password"
        type="password"
        style={{ marginBottom: "20px" }}
        value={Password || ""}
        onChange={(e) => setState({ ...state, Password: e.target.value })}
      />
      <Button
        color="primary"
        variant="contained"
        style={{ width: "30%" }}
        disabled={!UserName || !Password}
        onClick={() => handleLogin()}
      >
        Validate
      </Button>
    </div>
  );
}
