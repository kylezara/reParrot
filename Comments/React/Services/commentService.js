import axios from "axios";
import {
  onGlobalError,
  onGlobalSuccess,
  API_HOST_PREFIX,
} from "./serviceHelpers";

const endpoint = `${API_HOST_PREFIX}/api/comments`;

let selectByEntityId = (entityId, entityTypeId) => {
  const config = {
    method: "GET",
    url: `${endpoint}?entityId=${entityId}&entityTypeId=${entityTypeId}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let addComment = (payload) => {
  const config = {
    method: "POST",
    url: endpoint,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let updateComment = (payload, id) => {
  const config = {
    method: "PUT",
    url: `${endpoint}/${id}`,
    withCredentials: true,
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config)
    .then(() => {
      return id;
    })
    .catch(onGlobalError);
};

let deleteComment = (id) => {
  const config = {
    method: "DELETE",
    url: `${endpoint}/${id}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config)
    .then(() => {
      return id;
    })
    .catch(onGlobalError);
};

const commentService = {
  selectByEntityId,
  addComment,
  updateComment,
  deleteComment,
};

export default commentService;
