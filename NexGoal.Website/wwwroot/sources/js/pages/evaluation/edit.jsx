import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { FormGroup, ControlLabel, FormControl, Button } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import { axiosError, resolveClientUrl } from '../../common-functions';
import { PerformanceRecordStatus } from '../../constants';
import getResource from '../../resourceHelper';

export default class EditMemberCheckIn extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceRecord: {
                performanceRecordId: document.getElementById('PerformanceRecordId').value,
                employee: null,
                grade: null,
                comment: '',
                projects: [],
                statusId: null
            },
            employeeOptions: [],
            performanceGradeOptions: [],
            projectOptions: [],
            errors: {}
        };

        this.handleEmployeeChange = this.handleEmployeeChange.bind(this);
        this.handleGradeChange = this.handleGradeChange.bind(this);
        this.handleCommentChange = this.handleCommentChange.bind(this);
        this.handleProjectChange = this.handleProjectChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Member/PerformanceCheckIn/GetMembers')),
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects')),
            axios(resolveClientUrl(`Member/PerformanceCheckIn/GetById/${this.state.performanceRecord.performanceRecordId}`))
        ];

        Promise.all(promises)
            .then(([employeeResponse, gradeResponse, projectResponse, performanceRecordResponse]) => {
                let employeeOptions = [];
                employeeResponse.data.map(item => {
                    employeeOptions.push({
                        value: item.employeeId,
                        label: `${item.fullName} (${item.nickName})`
                    });
                });
                const performanceGradeOptions = gradeResponse.data.map((performanceGrade) => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });
                const projectOptions = projectResponse.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });

                let performanceRecord = performanceRecordResponse.data;
                performanceRecord.employee = {
                    value: performanceRecord.employee.employeeId,
                    label: `${performanceRecord.employee.fullName} (${performanceRecord.employee.nickName})`
                };
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
                    employeeOptions: employeeOptions,
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions
                });
            }).catch(axiosError);
    }

    handleEmployeeChange(selectedOption) {
        let errors = { ...this.state.errors };
        delete errors['employee'];

        if (!selectedOption || isEmpty(selectedOption)) {
            errors.employee = getResource('EmployeeRequired');
        }

        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { employee: selectedOption }),
            errors: errors
        });
    }

    handleGradeChange(selectedOption) {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { grade: selectedOption })
        });
    }

    handleCommentChange(event) {
        let errors = { ...this.state.errors };
        delete errors['comment'];
        if (!event.target.value) {
            errors.comment = getResource('CommentRequired');
        }

        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { comment: event.target.value }),
            errors: errors
        });
    }

    handleProjectChange(selectedOptions) {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { projects: selectedOptions })
        });
    }

    validateForm() {
        let errors = {}, isValid = true;

        if (!this.state.performanceRecord.employee || isEmpty(this.state.performanceRecord.employee)) {
            errors.employee = getResource('EmployeeRequired');
            isValid = false;
        }

        if (!this.state.performanceRecord.comment || this.state.performanceRecord.comment.trim() === '') {
            errors.comment = getResource('CommentRequired');
            isValid = false;
        }

        this.setState({ errors });

        return isValid;
    }

    handleSubmit(event) {
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
                employeeId: this.state.performanceRecord.employee.value,
                gradeId: this.state.performanceRecord.grade ? this.state.performanceRecord.grade.value : null,
                comment: this.state.performanceRecord.comment,
                projectIds: this.state.performanceRecord.projects.map((project) => { return project.value; }).join(','),
                statusId: statusId
            });

            axios({
                url: resolveClientUrl('Member/PerformanceCheckIn/Edit'),
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
        let errors = this.state.errors;

        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Edit Performance Check-in</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <form className="form-filter">
                            {
                                !isEmpty(errors) &&
                                <ul className="list-message-error">
                                    {errors.employee && <li>{errors.employee}</li>}
                                    {errors.comment && <li>{errors.comment}</li>}
                                </ul>
                            }

                            <FormGroup>
                                <ControlLabel className="required-label">{getResource('Employee')}</ControlLabel>
                                <Select name="employee" classNamePrefix="react-select"
                                    className={`react-select-container employee-field ${!isEmpty(errors) && errors.employee && 'input-validation-error'}`}
                                    placeholder="Select Employee"
                                    value={this.state.performanceRecord.employee} options={this.state.employeeOptions}
                                    onChange={this.handleEmployeeChange} isClearable
                                />
                            </FormGroup>

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
                                <Button className="btn-cancel">{getResource('Cancel')}</Button>
                            </FormGroup>
                        </form>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<EditMemberCheckIn />, document.getElementById("evaluation-wrapper"));