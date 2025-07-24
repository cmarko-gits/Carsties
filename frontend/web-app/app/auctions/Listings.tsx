'use client';

import { useParamsStore } from "@/hooks/useParamsStore";
import qs from 'query-string';
import { useEffect, useState } from "react";
import { useShallow } from "zustand/react/shallow";
import { getData } from "../actions/auctionActions";
import AppPagination from "../components/AppPagination";
import EmptyFilter from "../components/EmptyFilter";
import { Auction, PagedResult } from "../types";
import AuctionCard from "./AuctionCard";
import Filters from "./Filters";

export default function Listings() {

  const [data,setData] = useState<PagedResult<Auction>>()

  const params = useParamsStore(useShallow((state) => ({
    pageNumber: state.pageNumber,
    pageSize: state.pageSize,
    searchTerm: state.searchTerm,
    orderBy:state.orderBy,
    filterBy:state.filterBy
  })));

  const setParams = useParamsStore(state=>state.setParams)
const url = qs.stringify(params, { skipEmptyString: true, skipNull: true });
  
  function setPageNumber(pageNumber:number){
    setParams({pageNumber})
  }

  useEffect(() => {
    getData(url).then(data=>{
      setData(data)
    })
  }, [url]);
  
  return (
    <>
      <Filters />

      {data?.totalCount===0 ? (
        <EmptyFilter/>
      ):(
        <>
          <div className="grid grid-cols-4 gap-6">
        {data && data.result.map(auction => (
          <AuctionCard key={auction.id} auction={auction} />
        ))}
      </div>

        {data && data.pageCount > 0 && (
      <div className="flex justify-center mt-4">
        <AppPagination
          currentPage={params.pageNumber}
          pageCount={data.pageCount}
          pageChanged={setPageNumber}
        />
      </div>
    )}
        </>
      )}

      

          
    </>
  );
}
