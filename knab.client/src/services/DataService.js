import { apiPaths } from "../config";
import { setLoader, setSnackbar } from "../state/displaySlice";
import store from "../store";

class _DataService {
  GetCurrencies = () => {
    return new Promise((resolve, reject) => {
      fetch(apiPaths.getCurrencies, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${store.getState().auth.jwt}`,
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
          resolve(result);
        })
        .catch((err) => reject(err));
    });
  };

  GetDataForCurrency = (currency) => {
    return new Promise((resolve, reject) => {
      fetch(`${apiPaths.getCurrencyData}/${currency}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${store.getState().auth.jwt}`,
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
          resolve(result);
        })
        .catch((err) => reject(err));
    });
  };
}

export const DataService = new _DataService();
