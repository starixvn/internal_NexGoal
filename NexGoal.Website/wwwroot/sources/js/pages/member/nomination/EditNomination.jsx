import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { FormGroup, ControlLabel, Button, Modal } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';
import { Formik } from 'formik';
import moment from 'moment'

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { DateFormat, PerformanceRecordStatus } from '../../../constants';
import getResource from '../../../resourceHelper';
import TextareaFullscreenEditor from '../../../shared/components/textarea-fullscreen/index';

export default class EditNomination extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nominationPeriodOptions: props.nominationPeriods.map(np => {
                return { value: np.nominationPeriodId, label: np.title }
            }),
            createdDate: moment(props.performanceRecord.createdDate),
            eoqQuestions: [],
            eoyQuestions: [],
            initialObj: {
                period: {
                    label: props.performanceRecord.nomination.period.title,
                    value: props.performanceRecord.nomination.period.nominationPeriodId
                },
                employee: {
                    label: `${props.performanceRecord.employees[0].fullName} (${props.performanceRecord.employees[0].nickName})`,
                    value: props.performanceRecord.employees[0].employeeId
                },
                comment: props.performanceRecord.comment,
                statusId: props.performanceRecord.statusId
            }
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetEOQQuestions')),
            axios(resolveClientUrl('Common/GetEOYQuestions')),
        ];

        Promise.all(promises)
            .then(([eoqResponse, eoyResponse]) => {
                this.setState({
                    eoqQuestions: eoqResponse.data,
                    eoyQuestions: eoyResponse.data
                });
            }).catch(axiosError);
    }

    validateForm = values => {
        let errors = {};

        if (!values.comment) {
            errors.comment = 'Comment is required!';
        }

        return errors;
    }

    handleSubmit = (values, { setSubmitting, setErrors }) => {
        axios({
            url: resolveClientUrl(`MemberNomination/Edit`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                performanceRecordId: this.props.performanceRecord.performanceRecordId,
                nominationPeriodId: values.period.value,
                nomineeId: this.props.isEmployeeSelectable ? values.employee.value : this.props.nomineeId,
                performanceRecordStatusId: PerformanceRecordStatus.Submitted,
                comment: values.comment
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                this.handleCloseForm();
                this.props.onEditSuccess(data.objectId);
                toast.success(getResource('CreatedSuccessfully'));
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

            setSubmitting(false);
        }).catch(axiosError);
    }

    handleSubmitForm = () => {
        this.refs.form.setFieldValue('statusId', PerformanceRecordStatus.Submitted);
        this.refs.form.submitForm();
    }

    handleSaveDraft = () => {
        this.refs.form.setFieldValue('statusId', PerformanceRecordStatus.Draft);
        this.refs.form.submitForm();
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting }) => {
        let onPeriodChange = (value) => {
            var period = this.props.nominationPeriods.find(x => x.nominationPeriodId === value.value);
            let questions = '';
            if (period.quarter) {
                questions = this.state.eoqQuestions.map(e => `${e.resourceId}. ${e.value}`).join("\r\n\r\n");
            } else {
                questions = this.state.eoyQuestions.map(e => `${e.resourceId}. ${e.value}`).join("\r\n\r\n");
            }

            setFieldValue('period', value);
            setFieldValue('comment', questions);
        }

        return (
            <form onSubmit={handleSubmit} className="form-filter form-performance edit">
                <FormGroup>
                    <ControlLabel>{getResource('NominationPeriod')}</ControlLabel>
                    <Select name="period" className={`react-select-container ${touched.period && errors.period && 'input-validation-error'}`} classNamePrefix="react-select"
                        value={values.period} onChange={onPeriodChange}
                        options={this.state.nominationPeriodOptions}
                    />
                </FormGroup>

                <FormGroup>
                    <ControlLabel>{getResource('Employee')}</ControlLabel>
                    {this.props.isEmployeeSelectable ?
                        <Select name="employee" className={`react-select-container ${touched.employee && errors.employee && 'input-validation-error'}`} classNamePrefix="react-select"
                            value={values.employee} onChange={value => setFieldValue('employee', value)}
                            options={this.props.employeeOptions}
                        />
                        :
                        values.employee.label
                    }

                </FormGroup>

                <FormGroup>
                    <ControlLabel className="required-label">{getResource('Comment')}</ControlLabel>
                    <TextareaFullscreenEditor
                        className={`form-control comment-field ${touched.comment && errors.comment && 'input-validation-error'}`}
                        name="comment"
                        value={values.comment}
                        onChange={handleChange} />
                </FormGroup>
            </form>
        );
    }

    render() {
        let currentUser = this.props.currentUser;

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md form-performance edit"
            >
                <Modal.Header>
                    <Modal.Title>{getResource('EditNomination')}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="record record-your-mind">
                        <div className="avatar-user">
                            <img src={currentUser.avatar} className="avatar" />
                            <div className="person-info">
                                <span className="role-member">{currentUser.nickName}</span>
                                <div className="submit-date">{this.state.createdDate.format(DateFormat)}</div>
                            </div>
                        </div>
                        <div className="comment">
                            <Formik enableReinitialize={true} initialValues={this.state.initialObj} validate={this.validateForm} onSubmit={this.handleSubmit} render={this.renderForm} ref="form" />
                        </div>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Cancel')}</Button>
                    {
                        this.props.performanceRecord.statusId === PerformanceRecordStatus.Draft &&
                        <Button type="submit" className="btn-cancel" name="saveAsDraft" onClick={this.handleSaveDraft}>{getResource('SaveAsDraft')}</Button>
                    }
                    <Button type="submit" bsStyle="primary" name="save" onClick={this.handleSubmitForm}>{getResource('Submit')}</Button>
                </Modal.Footer>
            </Modal>

        );
    }
}