import React, { useState, useEffect, useCallback } from "react";
import { Trash2, Edit, PenTool } from "react-feather";
import "./comment.css";
import PropTypes from "prop-types";
import { Formik, Form, Field, ErrorMessage } from "formik";
import commentSchema from "schemas/commentSchema";
import debug from "sabio-debug";
import Swal from "sweetalert2";

const _logger = debug.extend("SingleComment");

function SingleComment({
  aComment,
  currentUser,
  onUpdateClick,
  onDeleteRequested,
  onAddReply,
}) {
  const [pageData, setPageData] = useState({
    arrayOfReplies: [],
    replyMapped: [],
  });

  const [commentPayload, setCommentPayload] = useState({
    subject: aComment?.subject,
    text: aComment?.text,
    id: aComment?.id,
    parentId: aComment?.parentId,
    entityTypeId: 1,
    entityId: aComment.entityId,
  });

  const [replyPayload, setReplyPayload] = useState({
    subject: aComment?.subject,
    text: "",
    parentId: aComment?.id,
    entityTypeId: 1,
    entityId: aComment.entityId,
  });

  const [showEdit, setShowEdit] = useState(false);

  const [showReplyForm, setShowReplyForm] = useState(false);

  useEffect(() => {
    setPageData((prevState) => {
      const commentData = { ...prevState };
      commentData.arrayOfReplies = aComment.replies;
      if (aComment.replies && aComment.replies.length > 0) {
        commentData.replyMapped = aComment.replies.map(mapReplies);
      }
      return commentData;
    });
  }, [aComment]);

  const mapReplies = (singleComment) => {
    return (
      <div key={"Reply_" + singleComment.id} className="comment-reply">
        <SingleComment
          aComment={singleComment}
          currentUser={currentUser}
          onUpdateClick={onUpdateClick}
          onDeleteRequested={onDeleteRequested}
          onAddReply={onAddReply}
        />
      </div>
    );
  };

  const onEditClick = () => {
    setShowEdit(!showEdit);
  };

  const onEditComment = ({ resetForm }) => {
    _logger(commentPayload, "From Update Click");
    setShowEdit(!showEdit);
    onUpdateClick(commentPayload);
    resetForm();
  };

  const onLocalDeleteClick = useCallback(() => {
    Swal.fire({
      title: "Delete Comment?",
      text: "You're about to permanently delete a comment!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete!",
    }).then((result) => {
      if (result.isConfirmed) {
        onDeleteRequested(aComment);
      }
    });
  }, []);

  const onReplyClick = () => {
    setShowReplyForm(!showReplyForm);
  };

  const onReplySubmit = (values, { resetForm }) => {
    _logger(values, "From Reply Submit");
    setShowReplyForm(!showReplyForm);
    onAddReply(values);
    resetForm();
  };

  const editChangeHandler = (e) => {
    const name = e.target.name;
    const value = e.target.value;
    setCommentPayload((comment) => {
      let newComment = { ...comment };
      newComment[name] = value;
      return newComment;
    });
  };

  const replyChangeHandler = (e) => {
    const name = e.target.name;
    const value = e.target.value;
    setReplyPayload((comment) => {
      let newComment = { ...comment };
      newComment[name] = value;
      return newComment;
    });
  };

  return (
    <React.Fragment>
      <div className="comment-container">
        <div className="comment-card">
          <img
            className="comment-avatar rounded-circle avatar-md"
            src={aComment.author.avatarUrl}
            alt="User Avatar"
          />
          <strong className="comment-name">
            {" "}
            {aComment.author.firstName} {aComment.author.lastName}
          </strong>
          <p className="comment-text" type="text">
            {" "}
            {aComment.text}
          </p>

          <Formik
            enableReinitialize={true}
            initialValues={commentPayload}
            validationSchema={commentSchema}
            onSubmit={onEditComment}
          >
            <Form>
              <div className="form-group">
                {showEdit ? (
                  <div className="form-update">
                    <Field
                      className="form-control"
                      component="textarea"
                      name="text"
                      value={commentPayload.text}
                      onChange={editChangeHandler}
                      placeholder="Update comment..."
                    />
                    <button
                      type="submit"
                      className="btn-update btn btn-sm btn-warning"
                    >
                      Update
                    </button>
                    <button
                      type="button"
                      className="btn-cancel btn text-danger"
                      onClick={onEditClick}
                    >
                      Cancel
                    </button>
                    <ErrorMessage
                      name="text"
                      className="update-msg btn text-danger"
                    />
                  </div>
                ) : null}
              </div>

              {currentUser.roles.includes("SysAdmin") ||
              currentUser.roles.includes("OrgAdmin") ||
              currentUser.id === aComment.author.id ? (
                <Trash2
                  color="red"
                  size="22px"
                  type="button"
                  className="delete-item-icon-comment m-1"
                  onClick={onLocalDeleteClick}
                />
              ) : null}

              {currentUser.id === aComment.author.id ? (
                <Edit
                  color="grey"
                  size="22px"
                  type="button"
                  className="edit-item-icon-comment m-1"
                  onClick={onEditClick}
                />
              ) : null}

              <PenTool
                color="blue"
                size="22px"
                type="button"
                className="edit-item-icon-comment m-1"
                onClick={onReplyClick}
              />
            </Form>
          </Formik>

          <Formik
            enableReinitialize={true}
            initialValues={replyPayload}
            validationSchema={commentSchema}
            onSubmit={onReplySubmit}
          >
            <Form>
              <div className="form-group">
                {showReplyForm ? (
                  <div className="form-reply">
                    <Field
                      className="form-control"
                      component="textarea"
                      name="text"
                      value={replyPayload.text}
                      onChange={replyChangeHandler}
                      placeholder="Add a reply..."
                    />
                    <button
                      type="submit"
                      className="btn-update btn btn-sm btn-success"
                    >
                      add reply
                    </button>
                    <button
                      type="button"
                      className="btn-cancel btn btn-sm btn-danger"
                      onClick={onReplyClick}
                    >
                      cancel
                    </button>
                    <ErrorMessage
                      name="text"
                      className="reply-msg text-danger"
                    />
                  </div>
                ) : null}
              </div>
            </Form>
          </Formik>
        </div>
      </div>

      <div className="reply-card">{pageData.replyMapped}</div>
    </React.Fragment>
  );
}

SingleComment.propTypes = {
  aComment: PropTypes.shape({
    id: PropTypes.number.isRequired,
    parentId: PropTypes.number,
    dateCreated: PropTypes.string,
    entityId: PropTypes.number.isRequired,
    entityTypeId: PropTypes.number.isRequired,
    entityType: PropTypes.shape({
      id: PropTypes.number.isRequired,
      name: PropTypes.string.isRequired,
    }),
    author: PropTypes.shape({
      id: PropTypes.number.isRequired,
      firstName: PropTypes.string.isRequired,
      lastName: PropTypes.string.isRequired,
      avatarUrl: PropTypes.string.isRequired,
    }),
    subject: PropTypes.string.isRequired,
    text: PropTypes.string.isRequired,
    replies: PropTypes.arrayOf(PropTypes.shape({ id: PropTypes.number })),
  }),
  currentUser: PropTypes.shape({
    id: PropTypes.number.isRequired,
    roles: PropTypes.string,
  }),
  onUpdateClick: PropTypes.func.isRequired,
  onDeleteRequested: PropTypes.func.isRequired,
  onAddReply: PropTypes.func.isRequired,
};

export default SingleComment;
