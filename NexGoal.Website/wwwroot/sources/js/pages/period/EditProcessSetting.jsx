import React, { Component, Fragment } from 'react';
import axios from 'axios';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { camelCase, range } from 'lodash';

import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';

export default class EditProcessSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            settings: []
        };

        this.remindDayList = range(0, 8);
        this.focusDayList = range(1, 8);
    }

    componentDidMount() {
        this.getSettings();
    }

    getSettings = () => {
        axios.get(resolveClientUrl('Period/GetPeriodProcessSetting/' + this.props.periodId))
            .then(response => {
                let data = response.data;
                this.setState({ settings: data });
            }).catch(axiosError);
    }

    handleRemindDayChange = e => {
        var element = e.target;
        var processId = parseInt(element.dataset.processid, 10);

        var settings = this.state.settings.slice(0);
        var process = settings.find(e => e.processId === processId);
        process.remindDay = element.value;

        this.setState({ settings: settings });
    }

    renderSetting = e => {
        return (
            <tr key={e.processId} >
                <td>{e.processName}</td>
                <td key={e.processId} className="text-center">
                    <select data-processid={e.processId} value={e.remindDay} onChange={this.handleRemindDayChange}>
                        {this.remindDayList.map(this.renderOptions)}
                    </select>
                </td>
            </tr>
        );
    }

    save = () => {
        axios({
            url: resolveClientUrl('period/editprocesssetting'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                settings: this.state.settings,
                periodId: this.props.periodId
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
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
                } else {
                    toast.error(data.message);
                }
            }
        }).catch(axiosError);
    }

    renderOptions = option => <option key={option} value={option}>{option}</option>

    render() {
        return (
            <Fragment>
                <table className="table table-bordered" style={{ marginTop: '12px', width: '700px' }}>
                    <thead>
                        <tr>
                            <th />
                            <th>{getResource('RemindDay')}</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            this.state.settings.map(this.renderSetting)
                        }

                    </tbody>
                </table>
                <br />
                <button type="button" className="btn btn-primary" onClick={this.save}>{getResource('Submit')}</button>
            </Fragment>
        );
    }
}