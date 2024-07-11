import { configureStore } from "@reduxjs/toolkit";
import authReducer from "./state/authSlice";
import displayReducer from "./state/displaySlice";

export default configureStore({
  reducer: {
    auth: authReducer,
    display: displayReducer,
  },
});
