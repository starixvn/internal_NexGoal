import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { FormGroup, ControlLabel, FormControl, Button, Checkbox, Row, Col } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { PerformanceRecordStatus } from '../../../constants';
import avatar from '../../../../styles/images/avatar1.jpg';
import ToastLayout from '../../../shared/layouts/ToastLayout';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import PerformanceRecord from '../../../shared/components/PerformanceRecord';

export default class CreateCheckIn extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceRecord: {
                grade: null,
                comment: '',
                projects: [],
                statusId: null
            },
            performanceGradeOptions: [],
            projectOptions: [],
            errors: {}
        };

        this.handleGradeChange = this.handleGradeChange.bind(this);
        this.handleCommentChange = this.handleCommentChange.bind(this);
        this.handleProjectChange = this.handleProjectChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl('Common/GetProjects'))
        ];

        Promise.all(promises)
            .then(([gradeResponse, projectResponse]) => {
                const performanceGradeOptions = gradeResponse.data.map((performanceGrade) => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });
                const projectOptions = projectResponse.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });

                this.setState({
                    performanceGradeOptions: performanceGradeOptions,
                    projectOptions: projectOptions
                });
            }).catch(axiosError);
    }

    handleGradeChange(selectedOption) {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { grade: selectedOption })
        });
    }

    handleCommentChange(event) {
        let errors = {};

        if (!event.target.value) {
            errors.comment = 'Comment is required!';
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

        if (!this.state.performanceRecord.comment || this.state.performanceRecord.comment.trim() === '') {
            errors.comment = 'Comment is required!';
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
                gradeId: this.state.performanceRecord.grade ? this.state.performanceRecord.grade.value : null,
                comment: this.state.performanceRecord.comment,
                projectIds: this.state.performanceRecord.projects.map((project) => { return project.value; }).join(','),
                statusId: statusId
            });

            axios({
                url: resolveClientUrl('Self/PerformanceCheckIn/Create'),
                method: 'POST',
                data: performanceRecord,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toast.success('Created successfully!');
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
        const avataruser = `/dist/styles/images/${avatar}`;
        let errors = this.state.errors;

        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">New Performance Check-in</li>
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
                                <ControlLabel>Grade</ControlLabel>
                                <Select name="grade" className="react-select-container grade-field" classNamePrefix="react-select"
                                    placeholder="Select Grade"
                                    value={this.state.performanceRecord.grade}
                                    options={this.state.performanceGradeOptions}
                                    isSearchable={false}
                                    isClearable
                                    onChange={this.handleGradeChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <ControlLabel className="required-label">Comment</ControlLabel>
                                <FormControl componentClass="textarea" name="comment"
                                    className={`comment-field ${!isEmpty(errors) && errors.comment && 'input-validation-error'}`}
                                    value={this.state.performanceRecord.comment} onChange={this.handleCommentChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <ControlLabel>Project Tag</ControlLabel>
                                <Select name="projects" isMulti closeMenuOnSelect={false} placeholder="Projects"
                                    className="react-select-container project-field" classNamePrefix="react-select"
                                    value={this.state.performanceRecord.projects}
                                    options={this.state.projectOptions}
                                    onChange={this.handleProjectChange}
                                />
                            </FormGroup>
                            <FormGroup>
                                <Button type="submit" bsStyle="primary" name="saveAsDraft" onClick={this.handleSubmit}>Save as draft</Button>
                                <Button type="submit" bsStyle="primary" name="save" onClick={this.handleSubmit}>Submit</Button>
                                <Button className="btn-cancel">Cancel</Button>
                            </FormGroup>
                        </form>

                        <Checkbox>Show Performance Record</Checkbox>
                        <div className="performance-record">
                            <PerformanceRecord
                                avatar={avataruser}
                                nickName="Oliver"
                                jobTitle="SE"
                                action="Nominated"
                                period="Employee Of The Quarter 2018"
                                finalDate="21 Jul 2018"
                                editDate="23 Jul 2018"
                                comment="Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                        Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five.
                                    Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five."
                                grade="Exceeds Expectations"
                                projects="Nexgoal, Alpha"
                            />
                            <PerformanceRecord
                                avatar={avataruser}
                                nickName="Oliver"
                                jobTitle="SE"
                                action="Nominated"
                                period="Employee Of The Year 2018"
                                finalDate="21 Dec 2018"
                                editDate="23 Dec 2018"
                                comment="Lorem ipsum is simply dummy text of the printing and typesetting industry. Lorem ipsum has been the industry's standard dummy text ever since the 1500s. when an unknown printer took a galley of the type specialmen book. It has survived not only five."
                                grade="Exceeds Expectations Plus"
                                projects="Nexgoal, Alpha, Nexbot"
                            />
                        </div>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<CreateCheckIn />, document.getElementById("self-evaluation-wrapper"));