'use client';

import { Pagination } from "flowbite-react"; // ✅ dodaj ovo

type Props = {
  currentPage: number;
  pageCount: number;
  pageChanged : (page:number) => void;
};

export default function AppPagination({ currentPage, pageCount , pageChanged}: Props) {

  return (
    <Pagination
      currentPage={currentPage}
        onPageChange={(page: number) => pageChanged(page)}
      totalPages={pageCount}
      layout="pagination"
      showIcons={true}
      className="text-blue-500 mb-6"
    />
  );
}
