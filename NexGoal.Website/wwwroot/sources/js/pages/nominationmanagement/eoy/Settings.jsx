import React, { Component } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import DatePicker from 'react-datepicker';
import moment from 'moment';
import axios from 'axios';
import { toast } from 'react-toastify';

import commonFunctions from '../../../common-functions';

class Settings extends Component {
    constructor(props) {
        super(props);

        this.state = {
            eoy: {
                eoqyId: 0,
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
        const eoyId = document.getElementById('EOYId').value;

        axios.get(commonFunctions.getRootUrl() + `EOY/GetById/${eoyId}`)
            .then(response => {
                let data = response.data;

                this.setState({
                    eoy: {
                        eoqyId: data.eoqyId,
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
        this.setState({ eoy: Object.assign({}, this.state.eoy, { [name]: value }) });
    }

    handleSubmit(event) {
        event.preventDefault();

        let eoy = {
            eoqyId: this.state.eoy.eoqyId,
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
            url: commonFunctions.getRootUrl() + 'EOY/Edit',
            method: 'POST',
            data: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'multipart/form-data'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success('Updated successfully!');
            } else {
                toast.error(data.message);
            }
        }).catch(commonFunctions.axiosError);
    }

    render() {
        const isHost = this.state.isHost;
        const eoy = this.state.eoy;

        return (
            <Form id="edit-eoq-form" horizontal onSubmit={this.handleSubmit}>
                <FormGroup>
                    <label className="col-sm-2 control-label">Year</label>
                    <div className="col-sm-2 field">
                        <select className="form-control" value={this.state.eoy.year} disabled>
                            <option value={eoy.year}>{this.state.eoy.year}</option>
                        </select>
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label required-label">Start Date For Nominator</label>
                    <div className="col-sm-2 field">
                        <DatePicker
                            dateFormat="DD MMM, YYYY"
                            selected={eoy.startDateForNominator}
                            onChange={this.handleStartDateForNominatorChange}
                            onChangeRaw={this.onChangeRaw}
                            name="startDateForNominator" className="form-control" disabled={!isHost} />
                    </div>
                    <label className="col-sm-2 control-label required-label">End Date For Nominator</label>
                    <div className="col-sm-2 field">
                        <DatePicker
                            dateFormat="DD MMM, YYYY"
                            selected={eoy.endDateForNominator}
                            onChange={this.handleEndDateForNominatorChange}
                            onChangeRaw={this.onChangeRaw}
                            name="endDateForNominator" className="form-control" disabled={!isHost} />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label required-label">Remind Date For Nominator</label>
                    <div className="col-sm-2 field">
                        <DatePicker
                            dateFormat="DD MMM, YYYY"
                            selected={eoy.remindDateForNominator}
                            onChange={this.handleRemindDateForNominatorChange}
                            onChangeRaw={this.onChangeRaw}
                            name="remindDateForNominator" className="form-control" disabled={!isHost} />
                    </div>
                    <label className="col-sm-2 control-label required-label">End Date For Committee</label>
                    <div className="col-sm-2 field">
                        <DatePicker
                            dateFormat="DD MMM, YYYY"
                            selected={eoy.endDateForCommittee}
                            onChange={this.handleEndDateForCommitteeChange}
                            onChangeRaw={this.onChangeRaw}
                            name="endDateForCommittee" className="form-control" disabled={!isHost} />
                    </div>
                </FormGroup>
                <FormGroup>
                    <label className="col-sm-2 control-label">Description</label>
                    <div className="col-sm-6 field">
                        <textarea className="form-control comment none-resize" rows="3" disabled={isHost ? false : true}></textarea>
                    </div>
                </FormGroup>
                {isHost &&
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