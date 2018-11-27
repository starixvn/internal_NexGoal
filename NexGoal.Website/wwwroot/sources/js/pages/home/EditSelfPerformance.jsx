import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Button, ControlLabel, Modal } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';
import TextareaFullscreenEditor from '../../shared/components/textarea-fullscreen/index';

export default class EditSelfPerformance extends Component {
    static propTypes = {
        performanceRecordId: PropTypes.number,
        performanceGradeOptions: PropTypes.arrayOf(PropTypes.object),
        projectOptions: PropTypes.arrayOf(PropTypes.object),
        currentUser: PropTypes.object,
        showModal: PropTypes.bool,
        onCloseModal: PropTypes.func,
        onSubmitSuccess: PropTypes.func
    };

    constructor(props) {
        super(props);

        this.state = {
            performanceRecord: {},
            errors: {}
        };
    }

    componentDidMount() {
        axios(resolveClientUrl(`Self/Performance/GetById/${this.props.performanceRecordId}`))
            .then((response) => {
                let performanceRecord = response.data;
                performanceRecord.grade = performanceRecord.grade ? {
                    value: performanceRecord.grade.gradeId,
                    label: performanceRecord.grade.description ?
                        `${performanceRecord.grade.description} (${performanceRecord.grade.name})`
                        : performanceRecord.grade.name
                } : null;
                performanceRecord.projects = performanceRecord.projects.map(project => {
                    return { value: project.projectId, label: project.name };
                });

                this.setState({ performanceRecord });
            }).catch(axiosError);
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

            let record = this.state.performanceRecord;

            let performanceRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                performanceRecordId: record.performanceRecordId,
                gradeId: record.grade ? record.grade.value : null,
                comment: record.comment,
                projectIds: record.projects.map((project) => { return project.value; }).join(','),
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

                    this.props.onSubmitSuccess(data.objectIds, statusId);
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
        let currentUser = this.props.currentUser;
        let performanceRecord = this.state.performanceRecord;
        let errors = this.state.errors;

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md form-performance edit"
            >
                <Modal.Header>
                    <Modal.Title>{getResource('EditPerformance')}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="record">
                        <div className="avatar-user">
                            <img src={currentUser.avatar} className="avatar" />
                            <div className="person-info">
                                <span className="role-member">{currentUser.nickName}</span>
                                <div className="submit-date">{moment().format(DateFormat)}</div>
                            </div>
                        </div>
                        <div className="comment">
                            <TextareaFullscreenEditor
                                className={`form-control comment-field ${!isEmpty(errors) && errors.comment ? 'input-validation-error' : ''}`}
                                name="comment"
                                value={performanceRecord.comment}
                                onChange={this.handleCommentChange}
                            />
                        </div>
                        <div className="grade">
                            <ControlLabel>{getResource('Grade')}</ControlLabel>
                            <Select name="grade" className="react-select-container grade-field" classNamePrefix="react-select"
                                placeholder="Select Grade"
                                value={performanceRecord.grade}
                                options={performanceGradeOptions}
                                isSearchable={false} isClearable
                                onChange={this.handleGradeChange}
                            />
                        </div>
                        <div className="project">
                            <ControlLabel>{getResource('ProjectTag')}</ControlLabel>
                            <Select name="projects" isMulti closeMenuOnSelect={false} placeholder="Projects"
                                className="react-select-container project-field" classNamePrefix="react-select"
                                value={performanceRecord.projects}
                                options={projectOptions}
                                onChange={this.handleProjectChange}
                            />
                        </div>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Cancel')}</Button>
                    {
                        performanceRecord.statusId === PerformanceRecordStatus.Draft &&
                        <Button type="submit" className="btn-cancel" name="saveAsDraft" onClick={this.handleSubmit}>{getResource('SaveAsDraft')}</Button>
                    }
                    <Button type="submit" bsStyle="primary" name="save" onClick={this.handleSubmit}>{getResource('Publish')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}