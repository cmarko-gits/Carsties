import { useParamsStore } from "@/hooks/useParamsStore";
import { Button } from "flowbite-react";
import Heading from "./Heading";

type Props = {
    title?:string;
    subtitle?:string;
    showReset?:boolean
}


export default function EmptyFilter({
    title = "No matches for this fillter",
    subtitle = "Try changing the filter or search term",
    showReset
}:Props) {

  const reset = useParamsStore(state=>state.reset)

  return (
    <div className="flex flex-col gap-2 items-center justify-center h-[40vh] shadow-lg">
         <Heading title={title} subtitle={subtitle} center/>
         <div className="mt-4">
            {showReset && (
                <Button outline onClick={reset}>
                    Remove Filters
                </Button>
            )}
         </div>
    </div>
  )
}
