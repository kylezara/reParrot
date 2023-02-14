import { lazy } from "react";

const Comments = lazy(() => import("../components/comments/Comments"));

const PageNotFound = lazy(() => import("../components/error/Error404"));

const comments = [
  {
    path: "/comments",
    name: "Comments",
    element: Comments,
    roles: ["SysAdmin", "OrgAdmin", "OrgMember", "Customer", "Blogger"],
    exact: true,
    isAnonymous: false,
  },
];

const errorRoutes = [
  {
    path: "*",
    name: "Error - 404",
    element: PageNotFound,
    roles: [],
    exact: true,
    isAnonymous: false,
  },
];

const allRoutes = [
  ...comments,
  ...errorRoutes,
];

export default allRoutes;
