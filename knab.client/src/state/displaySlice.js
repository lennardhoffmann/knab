import { createSlice } from "@reduxjs/toolkit";

export const displaySlice = createSlice({
  name: "counter",
  initialState: {
    currentView: null,
    showSnackbar: {
      show: false,
      message: "",
    },
    showLoader: false,
  },
  reducers: {
    setView: (state, action) => {
      state.currentView = action.payload;
    },
    setSnackbar: (state, action) => {
      state.showSnackbar = action.payload;
    },
    setLoader: (state, action) => {
      state.showLoader = action.payload;
    },
  },
});

// Action creators are generated for each case reducer function
export const { setView, setSnackbar, setLoader } = displaySlice.actions;

export default displaySlice.reducer;
