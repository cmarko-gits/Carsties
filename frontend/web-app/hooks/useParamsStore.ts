import { create } from "zustand";

type State = { 
    pageNumber:number;
    pageSize:number;
    pageCount:number;
    searchTerm:string;
    orderBy:string;
    filterBy:string
}

type Actions = {
    setParams:(params:Partial<State>) => void;
    reset: ()=>void;
}

const initialState:State = {
    pageNumber:1,
    pageSize:12,
    pageCount:1,
    searchTerm:'',
    orderBy:"make",
    filterBy:"live"
}

export const useParamsStore = create<State & Actions>((set)=>({
    ...initialState,
    setParams: (newParams: Partial<State>) => {
  set((state) => ({
    ...state,
    ...newParams,
    pageNumber: newParams.pageNumber ?? 1 // reset na 1 ako nije eksplicitno
  }));
}
,
    reset : () => set(initialState)
}))