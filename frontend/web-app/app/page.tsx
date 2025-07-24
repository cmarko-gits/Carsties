import Listings from "./auctions/Listings";
import NavBar from "./NavBar/NavBar";

export default function Home() {
  return (
    <div>
        <NavBar/>
        <main className="container mx-auto px-5 pt-10">
             <Listings/>
        </main>    </div>
  );
}
