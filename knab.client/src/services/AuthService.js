import { apiPaths } from "../config";
import { setJwt } from "../state/authSlice";
import { setLoader, setSnackbar } from "../state/displaySlice";
import store from "../store";

class _AuthService {
  Login = (credentials) => {
    return new Promise((resolve, reject) => {
      fetch(apiPaths.login, {
        method: "POST",
        body: JSON.stringify(credentials),
        headers: {
          "Content-Type": "application/json",
        },
      })
        .then((res) => {
          if (!res.ok) {
            return res.json().then((error) => {
              store.dispatch(
                setSnackbar({ show: true, message: error.message })
              );

              store.dispatch(setLoader(false));

              reject({
                status: res.status,
                message: error.message || "An error occurred",
              });
            });
          }
          return res.json();
        })
        .then((result) => {
          if (result) {
            store.dispatch(setJwt(result.token));
          }

          resolve();
        })
        .catch((err) => reject(err));
    });
  };
}

export const AuthService = new _AuthService();
