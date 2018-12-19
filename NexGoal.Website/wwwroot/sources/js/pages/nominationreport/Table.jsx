import React, { Component, Fragment } from 'react';
import Select, { components } from 'react-select';

import getResource from '../../resourceHelper';
import { NominationStatuses } from '../../constants';
import { getNominationClassName, noDataIndication } from '../../common-functions';

const SingleValue = ({ children, ...props }) => (
    <components.SingleValue {...props}>
        <span className={getNominationClassName(props.selectProps.value.value)} >{children}</span>
    </components.SingleValue>
);

export default class Table extends Component {
    constructor(props) {
        super(props);

        this.state = {
            nominationStatuses: NominationStatuses.map(x => ({
                value: x.value,
                label: x.text
            }))
        }

        this.selectComponents = { SingleValue };
    }

    componentDidMount() {
        let header = this.refs.stickyHeader;

        window.addEventListener('scroll', function (e) {
            if (window.scrollY > 168) {
                header.classList.add('sticky');
            } else {
                header.classList.remove('sticky');
            }
        });
    }

    handleShowNominatorComment = (nomination, performanceRecordId) => {
        this.props.onShowNominatorComment(nomination, performanceRecordId);
    }

    handleChangeStatus = (nominationId, status) => {
        this.props.onUpdateStatus(nominationId, status.value);
    }

    renderNomination = (nomination, index) => {
        let renderStatus = this.props.canUpdateNominationStatus ?
            <Select name="nominationPeriod" className="react-select-container" classNamePrefix="react-select"
                value={this.state.nominationStatuses.find(x => x.value === nomination.statusId)} options={this.state.nominationStatuses}
                isClearable={false} isSearchable={false} components={this.selectComponents}
                onChange={this.handleChangeStatus.bind(this, nomination.nominationId)}
            />
            :
            <span className={getNominationClassName(nomination.statusId)}>
                {NominationStatuses.find(x => x.value === nomination.statusId).text}
            </span>

        return (
            <tr key={nomination.nominationId}>
                <td className="text-center">{index + 1}</td>
                <td>{`${nomination.nominee.fullName} (${nomination.nominee.nickName})`}</td>
                <td>{nomination.nominee.groupName}</td>
                <td className="text-center">{nomination.nominators.length}</td>
                <td>
                    {nomination.nominators.map((n, i, arr) =>
                        <Fragment key={n.performanceRecordId}>
                            <a onClick={this.handleShowNominatorComment.bind(this, nomination, n.performanceRecordId)}>{`${n.creator.fullName} (${n.creator.nickName})`}</a>
                            {i < arr.length - 1 && ', '}
                        </Fragment>)
                    }
                </td>
                <td>{renderStatus}</td>
            </tr>
        )
    }

    render() {
        let content = this.props.tableData.length > 0 ? this.props.tableData.map(this.renderNomination)
            : <tr><td className="text-center" colSpan={6}>{noDataIndication()}</td></tr>

        return (
            <table className="table table-bordered table-striped">
                <colgroup>
                    <col className="order"></col>
                    <col className="nominee"></col>
                    <col className="team"></col>
                    <col className="vote"></col>
                    <col className="nominator"></col>
                    <col className="status"></col>
                </colgroup>
                <thead>
                    <tr ref='stickyHeader'>
                        <th className="order">{getResource('SymbolCount')}</th>
                        <th className="nominee">{getResource('Nominee')}</th>
                        <th className="team">{getResource('Team')}</th>
                        <th className="vote">{getResource('Vote')}</th>
                        <th className="nominator">{getResource('Nominator')}</th>
                        <th className="status">{getResource('Status')}</th>
                    </tr>
                </thead>
                <tbody>
                    {content}
                </tbody>
            </table>
        );
    }
}