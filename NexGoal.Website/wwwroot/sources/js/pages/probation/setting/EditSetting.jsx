import React, { Component } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import axios from 'axios';
import { stringify } from 'qs';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import { toast } from 'react-toastify';
import { Formik, getIn } from 'formik';
import { camelCase, isEmpty } from 'lodash';
import Select from 'react-select';
import { object, array } from 'yup';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { DateFormat, PerformanceProcess, InputErrorCssClass } from '../../../constants';
import getResource from '../../../resourceHelper';

export default class EditSetting extends Component {
    onSubmit = (values, { setErrors }) => {
        const settings = values.settings.map(p => {
            return {
                employeeId: p.employeeId,
                processId: p.processId,
                evaluatorId: p.evaluator.value,
                fromDate: p.fromDate.format('YYYY-MM-DD'),
                toDate: p.toDate.format('YYYY-MM-DD'),
            };
        });

        axios({
            url: resolveClientUrl(`Probation/EditSetting`),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                settings: settings,
                employeeId: this.props.probationer.employeeId
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                this.props.onEditSuccess();
                toast.success(getResource('UpdatedSuccessfully'));
            } else {
                if (data.errors) {
                    const errors = {};
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

    schema = object().shape({
        settings: array().of(Yup.object().shape({
            evaluator: object().required(),
            fromDate: object().required(),
            toDate: object().required()
        }))
    })

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting }) => {
        return (
            <Form id="edit-employee-settings" horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    {!isEmpty(errors) && !isEmpty(touched) &&
                        <ul className="list-message-error">
                            {errors.noSelect && <li>{errors.noSelect}</li>}
                        </ul>
                    }
                    <table className="table table-bordered">
                        <thead>
                            <tr>
                                <th />
                                <th className="col-sm-4">{getResource('Evaluator')}</th>
                                <th>{getResource('StartDate')}</th>
                                <th>{getResource('EndDate')}</th>
                            </tr>
                        </thead>
                        <tbody>
                            {values.settings.map((s, i) => (
                                <tr key={s.processId}>
                                    <td>{s.process}</td>
                                    <td>
                                        {
                                            s.processId !== PerformanceProcess.Self ?
                                                <Select name={`settings.${i}.evaluator`}
                                                    classNamePrefix="react-select"
                                                    className="react-select-container"
                                                    value={s.evaluator}
                                                    onChange={value => setFieldValue(`settings.${i}.evaluator`, value)}
                                                    options={this.props.employeeOptions} placeholder="-- Search an employee --"
                                                />
                                                : s.evaluator.label
                                        }
                                    </td>
                                    <td>
                                        <DatePicker autoComplete="off" dateFormat={DateFormat} selected={s.fromDate}
                                            onChange={value => setFieldValue(`settings.${i}.fromDate`, value)}
                                            name={`settings.${i}.fromDate`} maxDate={s.toDate}
                                            className={`form-control ${getIn(touched, `settings.${i}.fromDate`) && getIn(errors, `settings.${i}.fromDate`)
                                                && InputErrorCssClass}`}
                                        />
                                    </td>
                                    <td>
                                        <DatePicker autoComplete="off" dateFormat={DateFormat} selected={s.toDate}
                                            onChange={value => setFieldValue(`settings.${i}.toDate`, value)}
                                            name={`settings.${i}.toDate`} minDate={s.fromDate}
                                            className={`form-control ${getIn(touched, `settings.${i}.toDate`) && getIn(errors, `settings.${i}.toDate`)
                                                && InputErrorCssClass}`}
                                        />
                                    </td>
                                </tr>)
                            )}
                        </tbody>
                    </table>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn btn-cancel" onClick={this.props.onCloseModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" disabled={isSubmitting} className="btn btn-primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    }

    render() {
        const settings = this.props.probationer.settings.map(s => ({
            employeeId: s.employeeId,
            processId: s.processId,
            process: s.process,
            evaluator: {
                value: s.evaluatorId,
                label: `${s.evaluatorNickName} (${s.evaluatorFullName})`
            },
            fromDate: moment(s.fromDate),
            toDate: moment(s.toDate)
        }));

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.showModal}
                onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md"
            >
                <Modal.Header closeButton>
                    <Modal.Title>{getResource('EditEmployeeSetting')} of {this.props.probationer.nickName}</Modal.Title>
                </Modal.Header>
                <Formik
                    initialValues={{ settings }}
                    validationSchema={this.schema}
                    onSubmit={this.onSubmit}
                    render={this.renderForm}
                />
            </Modal>
        );
    }
}