import * as Yup from "yup";

const commentSchema = Yup.object().shape({
  text: Yup.string()
    .min(2)
    .required("Comment needs to be more than 2 characters"),
});

export default commentSchema;
