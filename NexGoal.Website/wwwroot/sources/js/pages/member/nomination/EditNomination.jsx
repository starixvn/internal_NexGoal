import React, { Component } from 'react';
import { FormGroup, ControlLabel, Button, Modal } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { camelCase } from 'lodash';
import { stringify } from 'qs';
import { Formik } from 'formik';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { DateFormat } from '../../../constants';
import getResource from '../../../resourceHelper';
import TextareaFullscreenEditor from '../../../shared/components/textarea-fullscreen/index';

export default class EditNomination extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoqQuestions: [],
            eoyQuestions: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Common/GetEOQQuestions')),
            axios(resolveClientUrl('Common/GetEOYQuestions'))
        ];

        Promise.all(promises)
            .then(([eoqResponse, eoyResponse]) => {
                this.setState({
                    eoqQuestions: eoqResponse.data,
                    eoyQuestions: eoyResponse.data
                });
            }).catch(axiosError);

        window.addEventListener('beforeunload', this.onLeavingPageEventListener);
    }

    componentWillUnmount() {
        window.removeEventListener('beforeunload', this.onLeavingPageEventListener);
    }

    onLeavingPageEventListener = event => {
        debugger;
        if (this.refs.form) {
            var isDirty = this.refs.form.getFormikComputedProps().dirty;
            if (isDirty) {
                event.returnValue = ' ';
                return ' ';
            }
        }
    }

    validateForm = values => {
        let errors = {};

        if (!values.comment || values.comment.trim() === '') {
            errors.comment = 'Comment is required!';
        }

        return errors;
    }

    handleSubmit = (values, { setErrors }) => {
        axios({
            url: resolveClientUrl(`MemberNomination/Edit`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                performanceRecordId: this.props.performanceRecord.performanceRecordId,
                nominationPeriodId: values.period.value,
                nomineeId: values.employee.value,
                comment: values.comment.trim()
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                this.props.onEditSuccess(this.props.performanceRecord.performanceRecordId);
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
        }).catch(axiosError);
    }

    handleSubmitForm = () => {
        this.refs.form.submitForm();
    }

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue }) => {
        let onPeriodChange = (value) => {
            let period = this.props.nominationPeriods.find(x => x.nominationPeriodId === value.value);
            let questions = '';
            if (period.quarter) {
                questions = this.state.eoqQuestions.map(e => `${e.resourceId}. ${e.value}`).join("\r\n\r\n");
            } else {
                questions = this.state.eoyQuestions.map(e => `${e.resourceId}. ${e.value}`).join("\r\n\r\n");
            }

            setFieldValue('period', value);
            setFieldValue('comment', questions);
        };
        let nominationPeriodOptions = this.props.nominationPeriods.map(period => {
            return { value: period.nominationPeriodId, label: period.title };
        });

        return (
            <form onSubmit={handleSubmit} className="form-filter form-performance edit">
                <FormGroup>
                    <div className="nomination-period">
                        <ControlLabel>{getResource('NominationPeriod')}</ControlLabel>
                        <Select name="period"
                            classNamePrefix="react-select"
                            className={`react-select-container ${touched.period && errors.period && 'input-validation-error'}`}
                            value={values.period} onChange={onPeriodChange}
                            options={nominationPeriodOptions}
                        />
                    </div>
                    <div className="employee">
                        <ControlLabel>{getResource('Employee')}</ControlLabel>
                        {
                            this.props.isEmployeeSelectable ?
                                <Select name="employee"
                                    classNamePrefix="react-select"
                                    className={`react-select-container ${touched.employee && errors.employee && 'input-validation-error'}`}
                                    value={values.employee} onChange={value => setFieldValue('employee', value)}
                                    options={this.props.employeeOptions}
                                />
                                :
                                values.employee.label
                        }
                    </div>
                </FormGroup>
                <FormGroup>
                    <ControlLabel>{getResource('Comment')}</ControlLabel>
                    <TextareaFullscreenEditor
                        className={`form-control comment-field ${touched.comment && errors.comment && 'input-validation-error'}`}
                        name="comment"
                        value={values.comment}
                        onChange={handleChange}
                    />
                </FormGroup>
            </form>
        );
    }

    render() {
        let currentUser = this.props.currentUser;
        let initialObj = {
            period: {
                label: this.props.performanceRecord.nominationPeriod.title,
                value: this.props.performanceRecord.nominationPeriod.nominationPeriodId
            },
            employee: this.props.employeeOptions.find(e => e.value === this.props.performanceRecord.employees[0].employeeId),
            comment: this.props.performanceRecord.comment
        };
        let createdDate = moment(this.props.performanceRecord.createdDate);

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
                                <div className="submit-date">{createdDate.format(DateFormat)}</div>
                            </div>
                        </div>
                        <div className="comment">
                            <Formik
                                initialValues={initialObj}
                                validate={this.validateForm}
                                onSubmit={this.handleSubmit}
                                render={this.renderForm}
                                ref="form"
                            />
                        </div>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" bsStyle="primary" name="save" onClick={this.handleSubmitForm}>{getResource('Publish')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}