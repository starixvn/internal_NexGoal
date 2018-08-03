import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Button, FormGroup, ControlLabel, Checkbox, Col, FormControl, Form } from 'react-bootstrap';
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
            settings: []
        };

        this.durationList = range(0, 30);
        this.remindDayList = range(0, 8);
        this.focusDayList = range(0, 8);
    }

    componentDidMount() {
        this.getSettings();
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

    render() {
        return (
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
                                <Checkbox data-processid={e.processId} defaultChecked={e.NoticeAtBegin} onChange={this.handleNoticeAtBeginChange} />
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
                </tbody>
            </table>
        );
    }
}