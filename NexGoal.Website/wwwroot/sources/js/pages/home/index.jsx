import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';

import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, PerformanceRecordStatus } from '../../constants';
import PerformanceRecord from '../../shared/components/PerformanceRecord';
import PersonalTimeline from './PersonalTimeline';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            performanceRecords: []
        };
    }

    componentDidMount() {
        axios(resolveClientUrl('Self/Performance/Index')).then(response => {
            this.setState({
                performanceRecords: response.data.performanceRecords
            });
        }).catch(axiosError);
    }

    handleAddNewPerformanceClick = () => {
        window.location = resolveClientUrl('Self/Performance/Create');
    }

    render() {
        const performanceRecords = this.state.performanceRecords;

        return (
            <Fragment>
                <PersonalTimeline />
                <input className="form-control add-new-performance" placeholder="What's on your mind?"
                    onClick={this.handleAddNewPerformanceClick}
                />
                {
                    performanceRecords.length > 0 &&
                    <div className="performance-record">
                        {
                            performanceRecords.map((performanceRecord) => {
                                let isHost = performanceRecord.statusId === PerformanceRecordStatus.Draft
                                    || performanceRecord.statusId === PerformanceRecordStatus.Submitted;
                                let editUrl = isHost ? `${resolveClientUrl('Self/Performance/Edit/') + performanceRecord.performanceRecordId}` : null;
                                let submittedDate = performanceRecord.createdDate ? moment(performanceRecord.createdDate).format(DateFormat) : '';
                                let modifiedDate = performanceRecord.modifiedDate ? moment(performanceRecord.modifiedDate).format(DateFormat) : '';

                                return (
                                    <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                                        avatar={performanceRecord.creator.base64Image}
                                        nickName={performanceRecord.creator.nickName}
                                        submittedDate={submittedDate} modifiedDate={modifiedDate}
                                        isHost={isHost}
                                        editUrl={editUrl}
                                        grade={performanceRecord.grade ? performanceRecord.grade.name : ''}
                                        comment={performanceRecord.comment}
                                        projects={performanceRecord.projects.map(project => { return project.name; }).join(', ')}
                                    />
                                );
                            })
                        }
                    </div>
                }
            </Fragment>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('main-container'));