import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import moment from 'moment';
import InfiniteScroll from 'react-infinite-scroller';
import { Scrollbars } from 'react-custom-scrollbars';

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

    handleCreateNominationSuccess = performanceRecordId => {
        axios.get(resolveClientUrl('MemberNomination/GetByRecordId'), {
            params: {
                performanceRecordId: performanceRecordId
            }
        }).then(response => {
            this.setState(prevState => ({
                nominations: [response.data, ...prevState.nominations]
            }));
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
                <Scrollbars style={{ height: '100%' }} autoHide={true}  >
                    <CreateNomination currentUser={this.props.currentUser} employeeOptions={this.props.employeeOptions} isEmployeeSelectable={!this.props.isMemberDetail}
                        nomineeId={this.props.nomineeId} onCreateSuccess={this.handleCreateNominationSuccess} />

                    {this.state.nominations.map(this.renderPerformanceRecord)}
                </Scrollbars>
            </div>
        );
    }
}