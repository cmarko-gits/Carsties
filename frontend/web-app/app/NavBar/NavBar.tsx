import Logo from "./Logo";
import Search from "./Search";

export default function NavBar() {
  return (
 

    <header className="cursor-pointer sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-md">
        <Logo/>
        <div>
          <Search/>
        </div>
        <div>Login</div>
    </header>
  )
}
