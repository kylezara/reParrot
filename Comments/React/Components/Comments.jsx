import React, { useState, useEffect, useCallback } from "react";
import commentService from "services/commentService";
import PropTypes from "prop-types";
import debug from "sabio-debug";
import SingleComment from "./SingleComment";
import { Formik, Form, Field, ErrorMessage } from "formik";
import toastr from "toastr";
import commentSchema from "schemas/commentSchema";

const _logger = debug.extend("Comments");

function Comments(props) {
  const [pageData, setPageData] = useState({
    arrayOfComments: [],
    commentMapped: [],
  });

  const [addCommentPayload] = useState({
    subject: "",
    text: "",
    parentId: 0,
    entityTypeId: 1,
    entityId: 1,
  });

  const [getComment] = useState({
    subject: "",
    text: "",
    parentId: null,
    entityTypeId: 1,
    entityId: 1,
  });

  const getCommentsByEntity = () => {
    commentService
      .selectByEntityId(getComment.entityId, getComment.entityTypeId)
      .then(onGetCommentSuccess)
      .catch(onGetCommentError);
  };

  useEffect(() => {
    getCommentsByEntity();
  }, []);

  const onGetCommentSuccess = (response) => {
    _logger(response, "onGetCommentSuccess");
    let commentsArr = response.items;
    setPageData((prevState) => {
      const pdata = { ...prevState };
      pdata.arrayOfComments = commentsArr;
      pdata.commentMapped = commentsArr.map(mapComment);
      return pdata;
    });
  };

  const onGetCommentError = (err) => {
    if (err.response.status === 500) {
      _logger(err);
      toastr.error("Error loading comments");
    }
  };

  const onUpdateClick = useCallback((values) => {
    commentService
      .updateComment(values, values.id)
      .then(onUpdateCommentSuccess)
      .catch(onUpdateCommentError);
  }, []);

  const onUpdateCommentSuccess = (response) => {
    _logger(response);
    getCommentsByEntity();
    toastr.success("Update Successful");
  };

  const onUpdateCommentError = (error) => {
    _logger(error, "Error from Comments");
    toastr.error("Update Error");
  };

  const onDeleteRequested = useCallback((aComment) => {
    _logger("onDeleteRequested", aComment);
    const deleteHandler = getDeleteSuccessHandler(aComment.id);
    commentService
      .deleteComment(aComment.id)
      .then(deleteHandler)
      .catch(OnDeleteError);
  }, []);

  const getDeleteSuccessHandler = (idToBeDeleted) => {
    return () => {
      getCommentsByEntity();
      setPageData((prevState) => {
        const pd = { ...prevState };
        pd.arrayOfComments = [...pd.arrayOfComments];
        const idxOf = pd.arrayOfComments.findIndex((comment) => {
          let result = false;
          if (comment.id === idToBeDeleted) {
            result = true;
          }
          return result;
        });
        if (idxOf >= 0) {
          pd.arrayOfComments.splice(idxOf, 1);
          pd.commentMapped = pd.arrayOfComments.map(mapComment);
        }
        _logger("state in deleteSuccessHandler", pd);
        return pd;
      });
      toastr.success("Comment deleted!");
    };
  };

  const OnDeleteError = (err) => {
    _logger(err, "Delete Error from Comments");
    toastr.error("Error deleting comment");
  };

  const mapComment = (aComment) => {
    _logger(aComment, "From Comments: mapComment");
    return (
      <SingleComment
        aComment={aComment}
        key={aComment.id}
        currentUser={props.currentUser}
        onUpdateClick={onUpdateClick}
        onDeleteRequested={onDeleteRequested}
        onAddReply={onAddReply}
      />
    );
  };

  const onAddReply = useCallback((values) => {
    commentService
      .addComment(values)
      .then(onAddReplySuccess)
      .catch(onAddReplyError);
  }, []);

  const onAddReplySuccess = (response) => {
    _logger(response, "From Add Reply Success");
    toastr.success("Reply Added");
    commentService
      .selectByEntityId(getComment.entityId, getComment.entityTypeId)
      .then(onGetCommentSuccess)
      .catch(onGetCommentError);
  };

  const onAddReplyError = (error) => {
    _logger(error, "From Add Comment Error");
    toastr.error("Error Posting Reply");
  };

  const onClickAddComment = (values, { resetForm }) => {
    _logger(values, "onClickAddComment");
    resetForm();
    commentService
      .addComment(values)
      .then(onAddCommentSuccess)
      .catch(onAddCommentError);
  };

  const onAddCommentSuccess = (response) => {
    _logger(response, "From Add Comment Success");
    toastr.success("Comment Posted!");
    commentService
      .selectByEntityId(getComment.entityId, getComment.entityTypeId)
      .then(onGetCommentSuccess)
      .catch(onGetCommentError);
  };

  const onAddCommentError = (err) => {
    toastr.error("Error Posting Comment");
    _logger(err);
  };

  return (
    <React.Fragment>
      <h1 className="mb-0">Join the Discussion</h1>

      <Formik
        enableReinitialize={true}
        initialValues={addCommentPayload}
        onSubmit={onClickAddComment}
        validationSchema={commentSchema}
      >
        <Form>
          <div className="row comment-form-main">
            <div className="col-sm-5 col-md-6 col-lg-8">
              <div className="form-group">
                <Field
                  type="text"
                  className="form-control"
                  id="textInput"
                  name="text"
                  placeholder="Add a comment..."
                />
                <ErrorMessage
                  name="text"
                  component="div"
                  className="has-error mt-1 text-danger"
                />
              </div>
            </div>
            <div className="col-sm-5 col-md-4 col-lg-3">
              <button type="submit" className="btn btn-primary">
                Comment
              </button>
            </div>
          </div>
        </Form>
      </Formik>

      <div className="comment-container">
        <div className="col-12 my-1 ">
          <div className="row">{pageData.commentMapped}</div>
        </div>
      </div>
    </React.Fragment>
  );
}

Comments.propTypes = {
  entity: PropTypes.shape({
    typeId: PropTypes.number.isRequired,
    id: PropTypes.number.isRequired,
  }),
  currentUser: PropTypes.shape({
    id: PropTypes.number.isRequired,
  }),
};

export default Comments;
