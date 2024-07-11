import { Button, MenuItem, TextField } from "@mui/material";
import { useState } from "react";
import { DataService } from "../../services";
import store from "../../store";
import { setLoader } from "../../state/displaySlice";
import { DataGrid } from "@mui/x-data-grid";
import "./_style.displayComponents.Shared.scss";

export default function CryptoDataComponent() {
  const [state, setState] = useState({
    cryptoCurrencies: null,
    selectedCurrency: null,
    dataForCurrency: null,
  });

  const { cryptoCurrencies, selectedCurrency, dataForCurrency } = state;

  const getCurrencies = () => {
    store.dispatch(setLoader(true));

    DataService.GetCurrencies()
      .then((res) => {
        setState({ ...state, cryptoCurrencies: res });
      })
      .then(() => {
        setTimeout(() => {
          store.dispatch(setLoader(false));
        }, 1500);
      });
  };

  const handleCurrencyChange = (event) => {
    setState({
      ...state,
      selectedCurrency: event.target.value,
      dataForCurrency: null,
    });
  };

  const getCurrencyData = () => {
    DataService.GetDataForCurrency(selectedCurrency)
      .then((res) => {
        const rows = Object.keys(res).map((key, index) => ({
          id: index + 1,
          currency: key,
          price: res[key].price,
          lastUpdated: new Date(res[key].last_updated).toLocaleString(), // Format the date
        }));

        console.log("ROWS", rows);

        setState({ ...state, dataForCurrency: rows });
      })
      .then(() => {
        setTimeout(() => {
          store.dispatch(setLoader(false));
        }, 1500);
      });
  };

  const columns = [
    { field: "currency", headerName: "Currency", width: 150 },
    { field: "price", headerName: "Price", width: 150 },
    { field: "lastUpdated", headerName: "Last Updated", width: 250 },
  ];

  return (
    <div className="containerStyling">
      {!cryptoCurrencies && (
        <>
          <label className="labelStyling">
            Here we will retrieve thedifferent crypto currencies. Click the
            button below to retrieve the currencies.
          </label>
          <Button
            variant="contained"
            onClick={() => getCurrencies()}
            style={{ marginBottom: "20px" }}
            disabled={cryptoCurrencies == null ? false : true}
          >
            Get Crypto Currencies
          </Button>
        </>
      )}
      {cryptoCurrencies && cryptoCurrencies.length && (
        <>
          <label className="labelStyling">
            From the following dropdown you can select a cryptocurrency for
            which you would like to retrieve the rates for.
          </label>
          <TextField
            select
            label="Select currency"
            helperText="Please select a crypto currency from the list"
            style={{ marginBottom: "20px" }}
            onChange={handleCurrencyChange}
          >
            {cryptoCurrencies.map((option) => {
              return (
                <MenuItem
                  key={`${option.symbol}-${option.slug}`}
                  value={option.symbol}
                >{`${option.symbol} (${option.slug})`}</MenuItem>
              );
            })}
          </TextField>
        </>
      )}
      {selectedCurrency && (
        <>
          <Button
            variant="contained"
            onClick={getCurrencyData}
            style={{ marginBottom: "20px" }}
            disabled={dataForCurrency == null ? false : true}
          >
            Get data for currency
          </Button>
        </>
      )}
      {dataForCurrency && (
        <>
          <label className="labelStyling">
            The following table displays the retrieved data for the selected
            currency
          </label>
          <DataGrid rows={dataForCurrency} columns={columns} />
        </>
      )}
    </div>
  );
}
