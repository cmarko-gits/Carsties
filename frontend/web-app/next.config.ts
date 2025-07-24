import withFlowbiteReact from "flowbite-react/plugin/nextjs";
import type { NextConfig } from "next";

const nextConfig: NextConfig = {

  logging : {
    fetches:{
      fullUrl:true
    } 
  },
  images : {
    remotePatterns : [
      {
        protocol : 'https' , hostname: 'cdn.pixabay.com'
      }
    ]
  }
  
  }


export default withFlowbiteReact(nextConfig);
