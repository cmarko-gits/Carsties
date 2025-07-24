'use client'
import { useParamsStore } from "@/hooks/useParamsStore";
import { ChangeEvent, useEffect, useState } from "react";
import { FaSearch } from "react-icons/fa";

export default function Search() {

  const setParams = useParamsStore(state=>state.setParams)
  const [value , setValue] = useState('')
  const searchTerm = useParamsStore(state=>state.searchTerm)
  function handleChange(e:ChangeEvent<HTMLInputElement>){
    setValue(e.target.value)
  }

  useEffect(()=>{
    if(searchTerm==='') setValue('')
  },[searchTerm])

  function handleSearch(){
    setParams({searchTerm:value})
  }

  return (
    <div className='flex w-full max-w-2xl items-center border-2 border-gray-300 rounded-full px-4 py-2 shadow-sm'>
      <input 
        onKeyDown={(e)=>{
            if(e.key === "Enter"){
                handleSearch()
            }
        }}
        onChange={handleChange}
        type='text' 
        value={value}
        placeholder='Search for cars by make, model or color' 
        className="flex-grow w-full pl-2 bg-transparent focus:outline-none border-transparent focus:ring-0 text-sm text-gray-600"
      />
      <button onClick={handleSearch}>
        <FaSearch size={34} className="bg-red-400 text-white rounded-full p-2 cursor-pointer text-[34px]" />
      </button>
    </div>
  );
}
