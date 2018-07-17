import React, { Component } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import axios from 'axios';

import commonFunctions from '../../../common-functions';

class Settings extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoq: {
                eoqyId: 0,
                quarter: 1,
                year: new Date().getFullYear(),
                startDateForNominator: null,
                endDateForNominator: null,
                remindDateForNominator: null,
                endDateForCommittee: null,
                description: null
            },
            isHost: document.getElementById('IsHost').checked
        };

        this.handleStartDateForNominatorChange = this.handleStartDateForNominatorChange.bind(this);
        this.handleEndDateForNominatorChange = this.handleEndDateForNominatorChange.bind(this);
        this.handleRemindDateForNominatorChange = this.handleRemindDateForNominatorChange.bind(this);
        this.handleEndDateForCommitteeChange = this.handleEndDateForCommitteeChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentWillMount() {
        this.getEOQById();
    }

    getEOQById() {
        let eoqyId = document.getElementById('EOQYId').value;
        axios.get(commonFunctions.getRootUrl() + `EOQ/GetById/${eoqyId}`)
            .then(response => {
                let data = response.data;

                this.setState({
                    eoq: {
                        eoqyId: data.eoqyId,
                        quarter: data.quarter,
                        year: data.year,
                        startDateForNominator: moment(data.startDateForNominator),
                        endDateForNominator: moment(data.endDateForNominator),
                        remindDateForNominator: moment(data.remindDateForNominator),
                        endDateForCommittee: moment(data.endDateForCommittee),
                        description: data.description
                    }
                });
            }).catch(commonFunctions.axiosError);
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
            eoqyId: this.state.eoq.eoqyId,
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
            url: commonFunctions.getRootUrl() + 'EOQ/Edit',
            method: 'POST',
            data: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'multipart/form-data'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
            } else {
            }
        }).catch(commonFunctions.axiosError);
    }

    render() {
        return (
            <Form id="edit-eoq-form" horizontal onSubmit={this.handleSubmit}>
                <FormGroup>
                    <label className="col-sm-2 control-label">Quarter</label>
                    <div className="col-sm-2 field">
                        <select className="form-control" value={this.state.eoq.quarter} disabled>
                            <option value="1">1st</option>
                            <option value="2">2nd</option>
                            <option value="3">3rd</option>
                            <option value="4">4th</option>
                        </select>
                    </div>
                    <label className="col-sm-2 control-label">Year</label>
                    <div className="col-sm-2 field">
                        <select className="form-control" value={this.state.eoq.year} disabled>
                            <option value={this.state.eoq.year}>{this.state.eoq.year}</option>
                        </select>
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label required-label">Start Date For Nominator</label>
                    <div className="col-sm-2 field">
                        <DatePicker
                            dateFormat="DD MMM, YYYY"
                            selected={this.state.eoq.startDateForNominator}
                            onChange={this.handleStartDateForNominatorChange}
                            onChangeRaw={this.onChangeRaw}
                            name="startDateForNominator"
                            className="form-control" />
                    </div>
                    <label className="col-sm-2 control-label required-label">End Date For Nominator</label>
                    <div className="col-sm-2 field">
                        <DatePicker
                            dateFormat="DD MMM, YYYY"
                            selected={this.state.eoq.endDateForNominator}
                            onChange={this.handleEndDateForNominatorChange}
                            onChangeRaw={this.onChangeRaw}
                            name="endDateForNominator"
                            className="form-control" />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label required-label">Remind Date For Nominator</label>
                    <div className="col-sm-2 field">
                        <DatePicker
                            dateFormat="DD MMM, YYYY"
                            selected={this.state.eoq.remindDateForNominator}
                            onChange={this.handleRemindDateForNominatorChange}
                            onChangeRaw={this.onChangeRaw}
                            name="remindDateForNominator"
                            className="form-control" />
                    </div>
                    <label className="col-sm-2 control-label required-label">End Date For Committee</label>
                    <div className="col-sm-2 field">
                        <DatePicker
                            dateFormat="DD MMM, YYYY"
                            selected={this.state.eoq.endDateForCommittee}
                            onChange={this.handleEndDateForCommitteeChange}
                            onChangeRaw={this.onChangeRaw}
                            name="endDateForCommittee"
                            className="form-control" />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label">Description</label>
                    <div className="col-sm-6 field">
                        <textarea className="form-control comment none-resize" rows="3"></textarea>
                    </div>
                </FormGroup>
                {this.state.isHost &&
                    <FormGroup>
                        <label className="col-sm-2 control-label"></label>
                        <div className="col-sm-6 field">
                        <Button type="submit" className="btn btn-primary">Submit</Button>
                        </div>
                    </FormGroup>
                }
            </Form>
        );
    }
}

export default Settings;