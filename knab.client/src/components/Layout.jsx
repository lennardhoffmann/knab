import { useSelector } from "react-redux";
import { CryptoData, Login, Welcome } from "./displayComponents";
import { BackdropComponent, SnackbarComponent } from "./miscComponents";
import "./_style.Layout.scss";

export default function LayoutComponent() {
  const currentView = useSelector((state) => state.display.currentView);

  const GetComponent = () => {
    switch (currentView) {
      case "Login":
        return <Login />;
      case "Data":
        return <CryptoData />;
      default:
        return <Welcome />;
    }
  };
  return (
    <div className="mainLayout">
      <div className="displayContainer">{GetComponent()}</div>
      <BackdropComponent />
      <SnackbarComponent />
    </div>
  );
}
