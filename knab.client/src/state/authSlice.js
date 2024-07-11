import { createSlice } from "@reduxjs/toolkit";

export const authSlice = createSlice({
  name: "counter",
  initialState: {
    jwt: null,
  },
  reducers: {
    setJwt: (state, action) => {
      state.jwt = action.payload;
    },
  },
});

// Action creators are generated for each case reducer function
export const { increment, decrement, incrementByAmount, setJwt } =
  authSlice.actions;

export default authSlice.reducer;
