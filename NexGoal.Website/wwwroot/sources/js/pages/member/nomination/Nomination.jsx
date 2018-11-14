import React, { Component } from 'react';
import axios from 'axios';
import moment from 'moment';
import { Scrollbars } from 'react-custom-scrollbars';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import { PerformanceRecordStatus } from '../../../constants';
import PerformanceRecord from '../../../shared/components/PerformanceRecord';
import CreateNomination from './CreateNomination';
import EditNomination from './EditNomination';

export default class Nomination extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nominations: [],
            nominationPeriods: [],
            showEditModal: false
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('MemberNomination/GetMemberNominations'), {
                params: {
                    memberId: this.props.isMemberDetail ? this.props.nomineeId : null
                }
            }),
            axios(resolveClientUrl('MemberNomination/GetNominationPeriodsForNominator'))
        ];

        Promise.all(promises)
            .then(([nominationResponse, nominationPeriodResponse]) => {
                this.setState({
                    nominations: nominationResponse.data,
                    nominationPeriods: nominationPeriodResponse.data
                });
            }).catch(axiosError);
    }

    sortNominationsDesc = (firstNomination, secondNomination) => {
        if (firstNomination.statusId === PerformanceRecordStatus.Draft && secondNomination.statusId !== PerformanceRecordStatus.Draft)
            return -1;

        if (firstNomination.statusId !== PerformanceRecordStatus.Draft && secondNomination.statusId === PerformanceRecordStatus.Draft)
            return 1;

        var firsNominationCreatedDate = moment(firstNomination.createdDate);
        var secondNominationCreatedDate = moment(secondNomination.createdDate);

        if (firsNominationCreatedDate > secondNominationCreatedDate)
            return -1;

        if (firsNominationCreatedDate < secondNominationCreatedDate)
            return 1;

        return 0;
    }

    handleCreateNominationSuccess = performanceRecordId => {
        axios.get(resolveClientUrl('MemberNomination/GetByRecordId'), {
            params: {
                performanceRecordId: performanceRecordId
            }
        }).then(response => {
            this.setState(prevState => {
                let nominations = [response.data, ...prevState.nominations];
                nominations.sort(this.sortNominationsDesc);

                return {
                    nominations: nominations
                };
            });
        }).catch(axiosError);
    }

    handleEditNominationSuccess = performanceRecordId => {
        axios.get(resolveClientUrl('MemberNomination/GetByRecordId'), {
            params: {
                performanceRecordId: performanceRecordId
            }
        }).then(response => {
            this.setState(prevState => {
                let nominations = prevState.nominations.slice();
                let index = nominations.findIndex(x => x.performanceRecordId === performanceRecordId);
                nominations[index] = response.data;
                nominations.sort(this.sortNominationsDesc);

                return {
                    nominations: nominations,
                    showEditModal: false
                };
            });
        }).catch(axiosError);
    }

    handleEdit = performanceRecordId => {
        this.setState(prevState => ({
            showEditModal: true,
            editedNomination: prevState.nominations.find(x => x.performanceRecordId === performanceRecordId)
        }));
    }

    closeEditModal = () => this.setState({ showEditModal: false, editedNomination: null });

    renderPerformanceRecord = (performanceRecord) => {
        return (
            <PerformanceRecord key={`performanceRecord${performanceRecord.performanceRecordId}`}
                data={performanceRecord}
                onEdit={this.handleEdit}
            />
        );
    }

    render() {
        return (
            <div className="record-list">
                <Scrollbars style={{ height: '100%' }} autoHide={true}>
                    {
                        this.state.nominationPeriods.length > 0 &&
                        <CreateNomination currentUser={this.props.currentUser} employeeOptions={this.props.employeeOptions}
                            nominationPeriods={this.state.nominationPeriods} isEmployeeSelectable={!this.props.isMemberDetail}
                            nomineeId={this.props.nomineeId} onCreateSuccess={this.handleCreateNominationSuccess}
                        />
                    }

                    {this.state.nominations.map(this.renderPerformanceRecord)}
                </Scrollbars>

                {this.state.showEditModal &&
                    <EditNomination showModal={this.state.showEditModal}
                        currentUser={this.props.currentUser}
                        performanceRecord={this.state.editedNomination}
                        nominationPeriods={this.state.nominationPeriods}
                        employeeOptions={this.props.employeeOptions}
                        onCloseModal={this.closeEditModal}
                        onEditSuccess={this.handleEditNominationSuccess}
                        isEmployeeSelectable={!this.props.isMemberDetail}
                    />
                }
            </div>
        );
    }
}