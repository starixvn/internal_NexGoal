import React, { Component } from 'react';
import { Button, Form, FormGroup, Modal } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import axios from 'axios';
import { toast } from 'react-toastify';

import commonFunctions from '../../../common-functions';

class CreateEOQ extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoq: {
                quarter: 1,
                year: new Date().getFullYear(),
                startDateForNominator: null,
                endDateForNominator: null,
                remindDateForNominator: null,
                endDateForCommittee: null,
                description: null
            }
        };

        this.handleQuarterChange = this.handleQuarterChange.bind(this);
        this.handleYearChange = this.handleYearChange.bind(this);
        this.handleStartDateForNominatorChange = this.handleStartDateForNominatorChange.bind(this);
        this.handleEndDateForNominatorChange = this.handleEndDateForNominatorChange.bind(this);
        this.handleRemindDateForNominatorChange = this.handleRemindDateForNominatorChange.bind(this);
        this.handleEndDateForCommitteeChange = this.handleEndDateForCommitteeChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleQuarterChange(event) {
        this.setState({ eoq: Object.assign({}, this.state.eoq, { quarter: event.target.value }) });
    }

    handleYearChange(event) {
        this.setState({ eoq: Object.assign({}, this.state.eoq, { year: event.target.value }) });
    }

    handleStartDateForNominatorChange(date) {
        this.updateEOQValue('startDateForNominator', date);
    }

    handleEndDateForNominatorChange(date) {
        this.updateEOQValue('endDateForNominator', date);
    }

    handleRemindDateForNominatorChange(date) {
        this.updateEOQValue('remindDateForNominator', date);
    }

    handleEndDateForCommitteeChange(date) {
        this.updateEOQValue('endDateForCommittee', date);
    }

    onChangeRaw(event) {
        let date = moment(event.target.value);
        if (date.isValid()) {
            this.updateEOQValue(event.target.name, date);
        }
    }

    updateEOQValue(name, value) {
        this.setState({ eoq: Object.assign({}, this.state.eoq, { [name]: value }) });
    }

    handleSubmit(event) {
        event.preventDefault();

        let eoq = {
            quarter: this.state.eoq.quarter,
            year: this.state.eoq.year,
            startDateForNominator: this.state.eoq.startDateForNominator !== null ? this.state.eoq.startDateForNominator.toDate() : null,
            endDateForNominator: this.state.eoq.endDateForNominator !== null ? this.state.eoq.endDateForNominator.toDate() : null,
            remindDateForNominator: this.state.eoq.remindDateForNominator !== null ? this.state.eoq.remindDateForNominator.toDate() : null,
            endDateForCommittee: this.state.eoq.endDateForCommittee !== null ? this.state.eoq.endDateForCommittee.toDate() : null,
            description: this.state.eoq.description
        };

        let formData = commonFunctions.objectToFormData(eoq);
        formData.append('__RequestVerificationToken', document.getElementsByName('__RequestVerificationToken')[0].value);

        axios({
            url: commonFunctions.getRootUrl() + 'EOQ/Create',
            method: 'POST',
            data: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'multipart/form-data'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Created successfully!');

                this.props.closeModal();
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    render() {
        const currentYear = new Date().getFullYear();
        const eoq = this.state.eoq;

        return (
            <Modal show={this.props.showModal} keyboard={false}
                onHide={this.props.closeModal} dialogClassName="custom-modal modal-md">
                <Modal.Header closeButton>
                    <Modal.Title>Create Nomination Period</Modal.Title>
                </Modal.Header>
                <Form id="create-eoq-form" horizontal onSubmit={this.handleSubmit}>
                    <Modal.Body>
                        <FormGroup>
                            <label className="col-sm-3 control-label required-label">Quarter</label>
                            <div className="col-sm-3 field">
                                <select className="form-control" onChange={this.handleQuarterChange}>
                                    <option value="1">1st</option>
                                    <option value="2">2nd</option>
                                    <option value="3">3rd</option>
                                    <option value="4">4th</option>
                                </select>
                            </div>
                            <label className="col-sm-3 control-label required-label">Year</label>
                            <div className="col-sm-3 field">
                                <select className="form-control" onChange={this.handleYearChange}>
                                    <option value={currentYear}>{currentYear}</option>
                                    <option value={currentYear + 1}>{currentYear + 1}</option>
                                </select>
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="col-sm-3 control-label required-label">Start Date For Nominator</label>
                            <div className="col-sm-3 field">
                                <DatePicker
                                    dateFormat="DD MMM, YYYY"
                                    selected={eoq.startDateForNominator}
                                    onChange={this.handleStartDateForNominatorChange}
                                    onChangeRaw={this.onChangeRaw}
                                    name="startDateForNominator"
                                    className="form-control" />
                            </div>
                            <label className="col-sm-3 control-label required-label">End Date For Nominator</label>
                            <div className="col-sm-3 field">
                                <DatePicker
                                    dateFormat="DD MMM, YYYY"
                                    selected={eoq.endDateForNominator}
                                    onChange={this.handleEndDateForNominatorChange}
                                    onChangeRaw={this.onChangeRaw}
                                    name="endDateForNominator"
                                    className="form-control" />
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="col-sm-3 control-label required-label">Remind Date For Nominator</label>
                            <div className="col-sm-3 field">
                                <DatePicker
                                    dateFormat="DD MMM, YYYY"
                                    selected={eoq.remindDateForNominator}
                                    onChange={this.handleRemindDateForNominatorChange}
                                    onChangeRaw={this.onChangeRaw}
                                    name="remindDateForNominator"
                                    className="form-control" />
                            </div>
                            <label className="col-sm-3 control-label required-label">End Date For Committee</label>
                            <div className="col-sm-3 field">
                                <DatePicker
                                    dateFormat="DD MMM, YYYY"
                                    selected={eoq.endDateForCommittee}
                                    onChange={this.handleEndDateForCommitteeChange}
                                    onChangeRaw={this.onChangeRaw}
                                    name="endDateForCommittee"
                                    className="form-control" />
                            </div>
                        </FormGroup>
                        <FormGroup>
                            <label className="col-sm-3 control-label">Description</label>
                            <div className="col-sm-9 field">
                                <textarea className="form-control comment none-resize" rows="5"></textarea>
                            </div>
                        </FormGroup>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={this.props.closeModal} className="btn-cancel">Cancel</Button>
                        <Button type="submit" className="btn btn-primary">Submit</Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        );
    }
}

export default CreateEOQ;