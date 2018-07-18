import React, { Component } from 'react';
import { Button, Form, FormGroup, Modal } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import axios from 'axios';
import { toast } from 'react-toastify';

import commonFunctions from '../../../common-functions';

class CreateEOY extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoy: {
                year: new Date().getFullYear(),
                startDateForNominator: null,
                endDateForNominator: null,
                remindDateForNominator: null,
                endDateForCommittee: null,
                description: null
            }
        };

        this.handleYearChange = this.handleYearChange.bind(this);
        this.handleStartDateForNominatorChange = this.handleStartDateForNominatorChange.bind(this);
        this.handleEndDateForNominatorChange = this.handleEndDateForNominatorChange.bind(this);
        this.handleRemindDateForNominatorChange = this.handleRemindDateForNominatorChange.bind(this);
        this.handleEndDateForCommitteeChange = this.handleEndDateForCommitteeChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleYearChange(event) {
        this.setState({ eoy: Object.assign({}, this.state.eoy, { year: event.target.value }) });
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
        this.setState({ eoy: Object.assign({}, this.state.eoy, { [name]: value }) });
    }

    handleSubmit(event) {
        event.preventDefault();

        let eoy = {
            quarter: this.state.eoy.quarter,
            year: this.state.eoy.year,
            startDateForNominator: this.state.eoy.startDateForNominator !== null ? this.state.eoy.startDateForNominator.toDate() : null,
            endDateForNominator: this.state.eoy.endDateForNominator !== null ? this.state.eoy.endDateForNominator.toDate() : null,
            remindDateForNominator: this.state.eoy.remindDateForNominator !== null ? this.state.eoy.remindDateForNominator.toDate() : null,
            endDateForCommittee: this.state.eoy.endDateForCommittee !== null ? this.state.eoy.endDateForCommittee.toDate() : null,
            description: this.state.eoy.description
        };

        let formData = commonFunctions.objectToFormData(eoy);
        formData.append('__RequestVerificationToken', document.getElementsByName('__RequestVerificationToken')[0].value);

        axios({
            url: commonFunctions.getRootUrl() + 'EOY/Create',
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
                this.props.handleFilterSubmit();
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    render() {
        const currentYear = new Date().getFullYear();
        const eoy = this.state.eoy;

        return (
            <Modal show={this.props.showModal} keyboard={false}
                onHide={this.props.closeModal} dialogClassName="custom-modal modal-md">
                <Modal.Header closeButton>
                    <Modal.Title>Create Nomination Period</Modal.Title>
                </Modal.Header>
                <Form id="create-eoy-form" horizontal onSubmit={this.handleSubmit}>
                    <Modal.Body>
                        <FormGroup>
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
                                    selected={eoy.startDateForNominator}
                                    onChange={this.handleStartDateForNominatorChange}
                                    onChangeRaw={this.onChangeRaw}
                                    name="startDateForNominator"
                                    className="form-control" />
                            </div>
                            <label className="col-sm-3 control-label required-label">End Date For Nominator</label>
                            <div className="col-sm-3 field">
                                <DatePicker
                                    dateFormat="DD MMM, YYYY"
                                    selected={eoy.endDateForNominator}
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
                                    selected={eoy.remindDateForNominator}
                                    onChange={this.handleRemindDateForNominatorChange}
                                    onChangeRaw={this.onChangeRaw}
                                    name="remindDateForNominator"
                                    className="form-control" />
                            </div>
                            <label className="col-sm-3 control-label required-label">End Date For Committee</label>
                            <div className="col-sm-3 field">
                                <DatePicker
                                    dateFormat="DD MMM, YYYY"
                                    selected={eoy.endDateForCommittee}
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

export default CreateEOY;