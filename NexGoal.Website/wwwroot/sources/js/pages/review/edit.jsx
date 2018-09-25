import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Button, Form, FormGroup } from 'react-bootstrap';
import axios from 'axios';
import { Formik } from 'formik';
import Select from 'react-select';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import PerformanceRecord from '../../shared/components/PerformanceRecord';

export default class EditPARecord extends Component {
    constructor(props) {
        super(props);

        this.state = {
            paRecord: null,
            paRecordId: document.getElementById('PARecordId').value,
            isHost: document.getElementById('IsHost').checked,
            performanceGradeOptions: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetPerformanceGrades')),
            axios(resolveClientUrl(`review/getPARecordById/${this.state.paRecordId}`))
        ];

        Promise.all(promises)
            .then(([gradeResponse, paRecordResponse]) => {
                const performanceGradeOptions = gradeResponse.data.map((performanceGrade) => {
                    return {
                        value: performanceGrade.gradeId,
                        label: performanceGrade.description ? `${performanceGrade.description} (${performanceGrade.name})` : performanceGrade.name
                    };
                });

                this.setState({
                    performanceGradeOptions: performanceGradeOptions,
                    paRecord: paRecordResponse.data
                });
            }).catch(axiosError);
    }

    handleOnSubmit = (values, { setErrors }) => {
        const paRecord = stringify({
            __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
            paRecordId: this.state.paRecordId,
            gradeId: values.grade.value,
            comment: values.comment
        });

        axios({
            url: resolveClientUrl('review/edit'),
            method: 'POST',
            data: paRecord,
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

                    setErrors(errors);
                } else {
                    toast.error(data.message);
                }
            }
        }).catch(axiosError);
    }

    validateForm(values) {
        let errors = {};

        if (!values.grade || !values.grade.value) {
            errors.gradeId = getResource('GradeRequired');
        }

        if (!values.comment) {
            errors.comment = getResource('CommentRequired');
        }

        return errors;
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        const isHost = this.state.isHost;
        const performanceGradeOptions = this.state.performanceGradeOptions;

        return (
            <Form id="edit-parecord-form" horizontal onSubmit={handleSubmit}>
                {
                    !isEmpty(errors) && !isEmpty(touched) &&
                    <ul className="list-message-error">
                        {touched.grade && errors.gradeId && <li>{errors.gradeId}</li>}
                        {touched.comment && errors.comment && <li>{errors.comment}</li>}
                    </ul>
                }

                <FormGroup>
                    <label className="col-sm-2 control-label required-label">{getResource('Grade')}</label>
                    <div className="col-sm-6">
                        <Select name="grade" classNamePrefix="react-select"
                            className={`react-select-container ${touched.grade && errors.gradeId && 'input-validation-error'}`}
                            placeholder="Select Grade"
                            value={values.grade}
                            options={performanceGradeOptions}
                            isSearchable={false} isClearable isDisabled={!isHost}
                            onChange={selectedOption => setFieldValue('grade', selectedOption)}
                        />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label required-label">{getResource('Comment')}</label>
                    <div className="col-sm-6">
                        <textarea className={`form-control none-resize ${touched.comment && errors.comment && 'input-validation-error'}`}
                            rows="3" maxLength="200"
                            name="comment" value={values.comment || ''}
                            onChange={handleChange} disabled={isHost ? false : true}
                        />
                    </div>
                </FormGroup>
                {isHost &&
                    <FormGroup>
                        <label className="col-sm-2 control-label" />
                        <div className="col-sm-6">
                            <Button type="submit" bsStyle="primary">{getResource('Submit')}</Button>
                        </div>
                    </FormGroup>
                }
            </Form>
        );
    }

    render() {
        const paRecord = this.state.paRecord;
        const processes = paRecord ? paRecord.processes : [];
        const record = {
            grade: paRecord ? paRecord.grade ? {
                value: paRecord.grade.gradeId,
                label: paRecord.grade.description ? `${paRecord.grade.description} (${paRecord.grade.name})` : paRecord.grade.name
            } : null : null,
            comment: paRecord ? paRecord.comment : null
        };

        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Review</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <div className="performance-record">
                            {
                                processes.map((process) => {
                                    return (
                                        process.performanceRecord &&
                                        <PerformanceRecord key={`process${process.performanceRecord.performanceRecordId}`}
                                            grade={process.performanceRecord.grade ? process.performanceRecord.grade.name : ''}
                                            comment={process.performanceRecord.comment}
                                            projects={process.performanceRecord.projects.map(project => { return project.name; }).join(',')}
                                        />
                                    );
                                })
                            }
                        </div>
                        <Formik enableReinitialize initialValues={record}
                            validate={this.validateForm}
                            onSubmit={this.handleOnSubmit}
                            render={this.renderForm}
                        />
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<EditPARecord />, document.getElementById('review-wrapper'));