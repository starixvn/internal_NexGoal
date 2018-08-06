import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { stringify } from 'qs';
import commonFunctions from '../../common-functions';
import { toast } from 'react-toastify';
import { camelCase, range } from 'lodash';
import TargetType from './TargetType';

export default class EditProcessSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            period: {},
            settings: []
        };

        this.durationList = range(1, 30);
        this.remindDayList = range(0, 8);
        this.focusDayList = range(1, 8);
    }

    componentDidMount() {
        this.getPeriod();
        this.getSettings();
    }

    getPeriod = () => {
        axios.get(commonFunctions.getRootUrl() + 'Period/Get/' + this.props.periodId)
            .then(response => {
                var data = response.data;
                this.setState({ period: data });
            }).catch(commonFunctions.axiosError);
    }

    getSettings = () => {
        axios.get(commonFunctions.getRootUrl() + 'Period/GetPeriodProcessSetting/' + this.props.periodId)
            .then(response => {
                let data = response.data;
                this.setState({ settings: data });
            }).catch(commonFunctions.axiosError);
    }

    handleDurationChange = e => {
        var element = e.target;
        var processId = element.dataset.processid;

        var settings = this.state.settings.slice(0);
        var process = settings.find(e => e.processId == processId);
        process.duration = element.value;

        this.setState({ settings: settings });
    }

    handleNoticeAtBeginChange = e => {
        var element = e.target;
        var processId = element.dataset.processid;

        var settings = this.state.settings.slice(0);
        var process = settings.find(e => e.processId == processId);
        process.noticeAtBegin = element.checked;

        this.setState({ settings: settings });
    }

    handleRemindDayChange = e => {
        var element = e.target;
        var processId = element.dataset.processid;

        var settings = this.state.settings.slice(0);
        var process = settings.find(e => e.processId == processId);
        process.remindDay = element.value;

        this.setState({ settings: settings });
    }

    handleFocusDayChange = e => {
        var element = e.target;
        var processId = element.dataset.processid;

        var settings = this.state.settings.slice(0);
        var process = settings.find(e => e.processId == processId);
        process.focusDay = element.value;

        this.setState({ settings: settings });
    }

    save = () => {
        axios({
            url: '/period/editprocesssetting',
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                settings: this.state.settings,
                periodId: this.props.periodId
            }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            let data = response.data;

            if (data.isSuccessful) {
                toast.success("Update Successfully");
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
        }).catch(commonFunctions.axiosError);
    }

    render() {
        return (
            <Fragment>
                <table className="table table-bordered">
                    <thead>
                        <tr>
                            <th></th>
                            {this.state.settings.map(e => <th key={e.processId} >{e.processName}</th>)}
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Duration</td>
                            {this.state.settings.map(e =>
                                <td key={e.processId} className="text-center">
                                    <select data-processid={e.processId} value={e.duration} onChange={this.handleDurationChange} >
                                        {this.durationList.map(dl => <option key={dl} value={dl}>{dl}</option>)}
                                    </select>
                                </td>)
                            }
                        </tr>
                        <tr>
                            <td>Notice At Begin</td>
                            {this.state.settings.map(e =>
                                <td key={e.processId} className="text-center">
                                    <input type="checkbox" data-processid={e.processId} checked={e.noticeAtBegin} onChange={this.handleNoticeAtBeginChange} />
                                </td>)
                            }
                        </tr>
                        <tr>
                            <td>Remind Day</td>
                            {this.state.settings.map(e =>
                                <td key={e.processId} className="text-center">
                                    <select data-processid={e.processId} value={e.remindDay} onChange={this.handleRemindDayChange}>
                                        {this.remindDayList.map(rl => <option key={rl} value={rl} >{rl}</option>)}
                                    </select>
                                </td>)
                            }
                        </tr>
                        {this.state.period.targetTypeId === TargetType.semiAnual &&
                            <tr>
                                <td>Focus Day</td>
                                {this.state.settings.map(e =>
                                    <td key={e.processId} className="text-center">
                                        {e.processId === 4 &&
                                            <select data-processid={e.processId} value={e.focusDay} onChange={this.handleFocusDayChange}>
                                                {this.focusDayList.map(fd => <option key={fd} value={fd} >{fd}</option>)}
                                            </select>
                                        }
                                    </td>)
                                }
                            </tr>
                        }
                    </tbody>
                </table>
                <br />
                <button type="button" className="btn btn-primary" onClick={this.save}>Save</button>
            </Fragment>
        );
    }
}