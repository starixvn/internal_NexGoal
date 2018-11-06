import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import InfiniteScroll from 'react-infinite-scroller';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { DateFormat, PerformanceRecordType } from '../../../constants';
import PerformanceRecord from '../../../shared/components/PerformanceRecord';
import CreateNomination from './CreateNomination';

export default class Nomination extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nominations: []
        }
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('MemberNomination/GetMemberNominations'), {
                params: {
                    memberId: this.props.isMemberDetail ? this.props.nomineeId : null
                }
            })
        ];

        Promise.all(promises)
            .then(([nominationResponse]) => {
                this.setState({
                    nominations: nominationResponse.data
                });
            }).catch(axiosError);
    }

    renderPerformanceRecord = (performanceRecord) => {
        let submittedDate = performanceRecord.createdDate ? moment(performanceRecord.createdDate).format(DateFormat) : '';
        let modifiedDate = performanceRecord.modifiedDate ? moment(performanceRecord.modifiedDate).format(DateFormat) : '';

        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                objectId={performanceRecord.performanceRecordId}
                employees={performanceRecord.employees}
                creator={performanceRecord.creator}
                roleId={performanceRecord.roleId}
                statusId={performanceRecord.statusId}
                submittedDate={submittedDate}
                modifiedDate={modifiedDate}
                isHost={performanceRecord.isHost}
                comment={performanceRecord.comment}
                onEdit={this.handleEditPerformanceRecord}
                type={PerformanceRecordType.Nomination}
                nomination={performanceRecord.nomination}
            />
        );
    }

    render() {
        return (
            <div className="record-list">
                <CreateNomination currentUser={this.props.currentUser} employeeOptions={this.props.employeeOptions} isEmployeeSelectable={!this.props.isMemberDetail} nomineeId={this.props.nomineeId} />

                {
                    this.state.nominations.length > 0 &&
                    this.state.nominations.map(this.renderPerformanceRecord)
                }
            </div>
        );
    }
}