import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Button, ControlLabel, FormControl, FormGroup, Modal } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';

export default class EditSelfPerformance extends Component {
    static propTypes = {
        performanceGradeOptions: PropTypes.arrayOf(PropTypes.object),
        projectOptions: PropTypes.arrayOf(PropTypes.object),
        onSubmitSuccess: PropTypes.func
    };

    constructor(props) {
        super(props);

        this.state = {
            performanceRecord: props.performanceRecord,
            errors: {}
        };
    }

    handleGradeChange = (selectedOption) => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { grade: selectedOption })
        });
    }

    handleCommentChange = (event) => {
        let errors = {};

        if (!event.target.value) {
            errors.comment = getResource('CommentRequired');
        }

        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { comment: event.target.value }),
            errors: errors
        });
    }

    handleProjectChange = (selectedOptions) => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { projects: selectedOptions })
        });
    }

    validateForm = () => {
        let errors = {}, isValid = true;

        if (!this.state.performanceRecord.comment || this.state.performanceRecord.comment.trim() === '') {
            errors.comment = getResource('CommentRequired');
            isValid = false;
        }

        this.setState({ errors });

        return isValid;
    }

    handleSubmit = (event) => {
        event.preventDefault();

        if (this.validateForm()) {
            let statusId = 0;
            switch (event.target.name) {
                case 'save':
                    statusId = PerformanceRecordStatus.Submitted;
                    break;
                case 'saveAsDraft':
                    statusId = PerformanceRecordStatus.Draft;
                    break;
            }

            let performanceRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                performanceRecordId: this.state.performanceRecord.performanceRecordId,
                gradeId: this.state.performanceRecord.grade ? this.state.performanceRecord.grade.value : null,
                comment: this.state.performanceRecord.comment,
                projectIds: this.state.performanceRecord.projects.map((project) => { return project.value; }).join(','),
                statusId: statusId
            });

            axios({
                url: resolveClientUrl('Self/Performance/Edit'),
                method: 'POST',
                data: performanceRecord,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toast.success(getResource('UpdatedSuccessfully'));
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
        let performanceGradeOptions = this.props.performanceGradeOptions;
        let projectOptions = this.props.projectOptions;
        let performanceRecord = this.state.performanceRecord;
        performanceRecord.grade = performanceRecord.grade ? {
            value: performanceRecord.grade.gradeId,
            label: performanceRecord.grade.description ?
                `${performanceRecord.grade.description} (${performanceRecord.grade.name})`
                : performanceRecord.grade.name
        } : null;
        performanceRecord.projects = performanceRecord.projects.map(project => {
            return { value: project.projectId, label: project.name };
        });
        let errors = this.state.errors;

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md"
            >
                <form className="form-filter form-performance">
                    <Modal.Body>
                        {
                            !isEmpty(errors) &&
                            <ul className="list-message-error">
                                {errors.comment && <li>{errors.comment}</li>}
                            </ul>
                        }

                        <FormGroup>
                            <ControlLabel>{getResource('Grade')}</ControlLabel>
                            <Select name="grade" className="react-select-container grade-field" classNamePrefix="react-select"
                                placeholder="Select Grade"
                                value={performanceRecord.grade}
                                options={performanceGradeOptions}
                                isSearchable={false} isClearable
                                onChange={this.handleGradeChange}
                            />
                        </FormGroup>
                        <FormGroup>
                            <ControlLabel className="required-label">{getResource('Comment')}</ControlLabel>
                            <FormControl componentClass="textarea" name="comment"
                                className={`comment-field ${!isEmpty(errors) && errors.comment && 'input-validation-error'}`}
                                value={performanceRecord.comment} onChange={this.handleCommentChange}
                            />
                        </FormGroup>
                        <FormGroup>
                            <ControlLabel>{getResource('ProjectTag')}</ControlLabel>
                            <Select name="projects" isMulti closeMenuOnSelect={false} placeholder="Projects"
                                className="react-select-container project-field" classNamePrefix="react-select"
                                value={performanceRecord.projects}
                                options={projectOptions}
                                onChange={this.handleProjectChange}
                            />
                        </FormGroup>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button type="submit" bsStyle="primary" name="saveAsDraft" onClick={this.handleSubmit}>{getResource('SaveAsDraft')}</Button>
                        <Button type="submit" bsStyle="primary" name="save" onClick={this.handleSubmit}>{getResource('Submit')}</Button>
                        <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Cancel')}</Button>
                    </Modal.Footer>
                </form>
            </Modal>
        );
    }
}