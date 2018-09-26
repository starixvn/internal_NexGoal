import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';

export default class EditSelfPerformance extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceRecord: {
                performanceRecordId: document.getElementById('PerformanceRecordId').value,
                grade: null,
                comment: '',
                projects: [],
                statusId: null
            },
            performanceGradeOptions: [],
            projectOptions: [],
            errors: {}
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl(`Self/Performance/GetById/${this.state.performanceRecord.performanceRecordId}`))
        ];

        Promise.all(promises)
            .then(([gradeResponse, projectResponse, performanceRecordResponse]) => {
                const performanceGradeOptions = gradeResponse.data.map(performanceGrade => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });
                const projectOptions = projectResponse.data.map(project => {
                    return { value: project.projectId, label: project.name };
                });
                let performanceRecord = performanceRecordResponse.data;
                performanceRecord.grade = {
                    value: performanceRecord.grade.gradeId,
                    label: performanceRecord.grade.description ?
                        `${performanceRecord.grade.description} (${performanceRecord.grade.name})`
                        : performanceRecord.grade.name
                };
                performanceRecord.projects = performanceRecord.projects.map(project => {
                    return { value: project.projectId, label: project.name };
                });

                this.setState({
                    performanceRecord: performanceRecord,
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions
                });
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

    handleCancel = () => {
        window.location = resolveClientUrl('');
    }

    render() {
        let errors = this.state.errors;

        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Edit Performance</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <form className="form-filter">
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
                                    value={this.state.performanceRecord.grade}
                                    options={this.state.performanceGradeOptions}
                                    isSearchable={false} isClearable
                                    onChange={this.handleGradeChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <ControlLabel className="required-label">{getResource('Comment')}</ControlLabel>
                                <FormControl componentClass="textarea" name="comment"
                                    className={`comment-field ${!isEmpty(errors) && errors.comment && 'input-validation-error'}`}
                                    value={this.state.performanceRecord.comment} onChange={this.handleCommentChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <ControlLabel>{getResource('ProjectTag')}</ControlLabel>
                                <Select name="projects" isMulti closeMenuOnSelect={false} placeholder="Projects"
                                    className="react-select-container project-field" classNamePrefix="react-select"
                                    value={this.state.performanceRecord.projects}
                                    options={this.state.projectOptions}
                                    onChange={this.handleProjectChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <Button type="submit" bsStyle="primary" name="saveAsDraft" onClick={this.handleSubmit}>{getResource('SaveAsDraft')}</Button>
                                <Button type="submit" bsStyle="primary" name="save" onClick={this.handleSubmit}>{getResource('Submit')}</Button>
                                <Button className="btn-cancel" onClick={this.handleCancel}>{getResource('Cancel')}</Button>
                            </FormGroup>
                        </form>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<EditSelfPerformance />, document.getElementById('self-performance-wrapper'));