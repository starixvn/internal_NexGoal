import React, { Component, Fragment } from 'react';
import { Button, ControlLabel, Modal, FormGroup } from 'react-bootstrap';
import Select from 'react-select';
import { camelCase, isEmpty } from 'lodash';
import { toast } from 'react-toastify';
import { stringify } from 'qs';
import TextareaFullscreenEditor from '../../../shared/components/textarea-fullscreen/index';
import getResource from '../../../resourceHelper';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../common-functions';

export default class EditMemberPA extends Component {
    constructor(props) {
        super(props);
        this.state = {
            paRecord: {
                paRecordId: this.props.paRecord.paRecordId,
                grade: this.props.paRecord.grade ?
                    this.props.gradeOptions.find(o => o.value === this.props.paRecord.grade.gradeId) : null,
                comment: this.props.paRecord.comment
            },
            errors: {}
        };
    }

    handleGradeChange = (selectedOption) => {
        this.setState({
            paRecord: Object.assign({}, this.state.paRecord, { grade: selectedOption })
        });
    }

    handleCommentChange = (event) => {
        let errors = { ...this.state.errors };
        delete errors['comment'];
        if (!event.target.value) {
            errors.comment = getResource('CommentRequired');
        }

        this.setState({
            paRecord: Object.assign({}, this.state.paRecord, { comment: event.target.value }),
            errors: errors
        });
    }

    validateForm = () => {
        let errors = {}, isValid = true;
        let paRecord = this.state.paRecord;

        if (!paRecord.comment || paRecord.comment.trim() === '') {
            errors.comment = getResource('CommentRequired');
            isValid = false;
        }

        if (!paRecord.grade || paRecord.grade.value <= 0) {
            errors.grade = getResource('GradeRequired');
            isValid = false;
        }

        this.setState({ errors });

        return isValid;
    }

    handleSubmit = (event) => {
        event.preventDefault();

        if (this.validateForm()) {
            let record = this.state.paRecord;

            let paRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                paRecordId: record.paRecordId,
                gradeId: record.grade ? record.grade.value : null,
                comment: record.comment
            });
            axios({
                url: resolveClientUrl('Review/Edit'),
                method: 'POST',
                data: paRecord,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toast.success(getResource('CreatedSuccessfully'));

                    this.props.onSubmitSuccess();
                } else {
                    if (data.errors) {
                        let errors = {};
                        data.errors.map(error => {
                            errors[camelCase(error.key)] = error.errorMessage;
                        });

                        this.setState({ errors });
                    } else {
                        toast.error(data.message);
                    }
                }
            }).catch(axiosError);
        }
    }
    render() {
        let paRecord = this.state.paRecord;
        let errors = this.state.errors;
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.isShowModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md form-performance edit"
            >
                <Modal.Header>
                    <Modal.Title>{getResource('EditPerformance')}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="record record-your-mind">
                        <div className="comment">
                            <form className="form-filter form-performance edit">
                                <FormGroup>
                                    <ControlLabel className="required-label">{getResource('Comment')}</ControlLabel>
                                    <TextareaFullscreenEditor
                                        className={`form-control comment-field ${!isEmpty(errors) && errors.comment ? 'input-validation-error' : ''}`}
                                        name="comment"
                                        value={paRecord.comment}
                                        onChange={this.handleCommentChange}
                                    />
                                </FormGroup>
                                <FormGroup>
                                    <div className="grade">
                                        <ControlLabel>{getResource('Grade')}</ControlLabel>
                                        <Select name="grade" classNamePrefix="react-select"
                                            className={`react-select-container grade-field ${!isEmpty(errors) && errors.grade ? 'input-validation-error' : ''}`}
                                            placeholder="Select Grade"
                                            value={paRecord.grade}
                                            options={this.props.gradeOptions}
                                            isSearchable={false}
                                            onChange={this.handleGradeChange}
                                        />
                                    </div>
                                </FormGroup>
                            </form>
                        </div>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" bsStyle="primary" name="save" onClick={this.handleSubmit}>{getResource('Submit')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}