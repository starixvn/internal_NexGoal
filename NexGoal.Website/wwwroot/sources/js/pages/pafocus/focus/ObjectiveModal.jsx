import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { Button, Form, FormGroup, Modal } from 'react-bootstrap';
import { Formik } from 'formik';
import axios from 'axios';
import { axiosError, resolveClientUrl, blockUI, unblockUI } from '../../../common-functions';
import { toast } from 'react-toastify';

import getResource from '../../../resourceHelper';

export default class ObjectiveModal extends Component {
    static propTypes = {
        isHost: PropTypes.bool,
        objective: PropTypes.objectOf(PropTypes.object),
        isShow: PropTypes.bool,
        onClose: PropTypes.func,
        onSubmit: PropTypes.func
    };

    getNextObjectives = setFieldValue => {
        blockUI();
        const { disciplineStartDate, disciplineEndDate, employee } = this.props;

        axios(resolveClientUrl('PAFocus/GetLastPerformanceRecordByEmployee'),
            {
                params: {
                    employeeId: employee.employeeId,
                    fromDate: disciplineStartDate,
                    toDate: disciplineEndDate
                }
            }).then(response => {
                const data = response.data;
                setTimeout(() => {
                    data && data.comment ? setFieldValue(`value`, data.comment) : toast.info('No data found!');
                    unblockUI();
                }, 300);
            }).catch(axiosError);
    }

    handleSubmit = values => {
        this.props.onClose();
        this.props.onSubmit(values);
    };

    validateForm = values => {
        const errors = {};

        if (!values.value) {
            errors.value = 'Required';
        }

        return errors;
    };

    renderForm = ({
        values,
        errors,
        touched,
        setFieldValue,
        handleChange,
        handleSubmit }) => {
        return (
            <Form id="objective-form" horizontal onSubmit={handleSubmit}>
                <Modal.Body>
                    <FormGroup>
                        <div className="col-sm-12">
                            <textarea className={`form-control ${touched.value && errors.value && 'input-validation-error'}`}
                                rows="20"
                                name="value" value={values.value}
                                onChange={handleChange}
                            />
                        </div>
                    </FormGroup>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onClose}>{getResource('Cancel')}</Button>
                    {
                        this.props.objective.name === "nextObjective" &&
                        <Button bsStyle="primary" onClick={() => this.getNextObjectives(setFieldValue)}>
                            {`Load ${this.props.employee.nickName}'s latest PR`}
                        </Button>
                    }
                    <Button type="submit" bsStyle="primary">{getResource('Submit')}</Button>
                </Modal.Footer>
            </Form>
        );
    };

    render() {
        const isHost = this.props.isHost;
        const modalTitle = `${isHost ? 'Edit ' : ''}${this.props.objective.name === 'currentObjective'
            ? getResource('AccomplishedCurrentObjectives') : getResource('NextObjectives')}`;

        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.isShow} onHide={this.props.onClose}
                dialogClassName="custom-modal modal-md"
            >
                <Modal.Header closeButton>
                    <Modal.Title>
                        {modalTitle}
                    </Modal.Title>
                </Modal.Header>
                {
                    isHost &&
                    <Formik
                        initialValues={this.props.objective}
                        validate={this.validateForm}
                        onSubmit={this.handleSubmit}
                        render={this.renderForm}
                    />
                }
                {
                    !isHost &&
                    <Fragment>
                        <Modal.Body>
                            <div className="comment">{this.props.objective.value}</div>
                        </Modal.Body>
                        <Modal.Footer>
                            <Button className="btn-cancel" onClick={this.props.onClose}>{getResource('Close')}</Button>
                        </Modal.Footer>
                    </Fragment>
                }
            </Modal>
        );
    }
}