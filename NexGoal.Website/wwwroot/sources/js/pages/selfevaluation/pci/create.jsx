import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { FormGroup, ControlLabel, FormControl, Button, Checkbox, Row, Col } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { Formik } from 'formik';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import commonFunctions from '../../../common-functions';
import avatar from '../../../../styles/images/avatar1.jpg';
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
            projectOptions: []
        };

        this.handleGradeChange = this.handleGradeChange.bind(this);
        this.handleProjectChange = this.handleProjectChange.bind(this);
    }

    componentDidMount() {
        this.getPerformanceGrades();
        this.getProjects();
    }

    getPerformanceGrades() {
        axios(commonFunctions.getRootUrl() + 'Common/GetPerformanceGrades')
            .then(response => {
                let performanceGradeOptions = response.data.map((performanceGrade) => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });

                this.setState({
                    performanceGradeOptions: performanceGradeOptions
                });
            });
    }

    getProjects() {
        axios(commonFunctions.getRootUrl() + 'Common/GetProjects')
            .then(response => {
                const projectOptions = response.data.map((project) => {
                    return { value: project.projectId, label: project.name };
                });

                this.setState({
                    projectOptions: projectOptions
                });
            });
    }

    handleGradeChange(selectedOption) {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { grade: selectedOption })
        });
    }

    handleProjectChange(selectedOptions) {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { projects: selectedOptions })
        });;
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        return (
            <form horizontal="true" className="form-filter" onSubmit={handleSubmit}>
                {
                    !isEmpty(errors) && !isEmpty(touched) &&
                    <ul className="list-message-error">
                        {touched.comment && errors.comment && <li>{errors.comment}</li>}
                    </ul>
                }

                <FormGroup>
                    <ControlLabel>Grade</ControlLabel>
                    <Select name="grade" className="react-select-container grade-field" classNamePrefix="react-select"
                        placeholder="Select Grade"
                        value={values.grade}
                        options={this.state.performanceGradeOptions}
                        isSearchable={false}
                        isClearable
                        onChange={this.handleGradeChange} />
                </FormGroup>
                <FormGroup>
                    <ControlLabel className="required-label">Comment</ControlLabel>
                    <FormControl componentClass="textarea" className="comment-field"
                        className={`comment-field ${touched.comment && errors.comment && 'input-validation-error'}`} />
                </FormGroup>
                <FormGroup>
                    <ControlLabel>Project Tag</ControlLabel>
                    <Select name="projects" isMulti closeMenuOnSelect={false} placeholder="Projects"
                        className="react-select-container project-field" classNamePrefix="react-select"
                        value={this.state.performanceRecord.projects}
                        options={this.state.projectOptions}
                        onChange={this.handleProjectChange} />
                </FormGroup>
                <FormGroup>
                    <Button type="submit" bsStyle="primary" className="btn-primary">Save as draft</Button>
                    <Button type="submit" bsStyle="primary" className="btn-primary">Submit</Button>
                    <Button className="btn-cancel">Cancel</Button>
                </FormGroup>
            </form>
        );
    }

    render() {
        const avataruser = `/dist/${avatar}`;

        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">New Performance Check-in</li>
                </Breadcrumb>
                <div className="col-sm-12">

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
        );
    }
}

ReactDOM.render(<CreateCheckIn />, document.getElementById("self-evaluation-wrapper"));