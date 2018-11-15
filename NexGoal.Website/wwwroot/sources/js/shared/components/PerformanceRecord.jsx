import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';

import { getNominationClassName, getRoleClassName, resolveClientUrl } from '../../common-functions';
import { DateFormat, NominationStatuses, PerformanceRecordStatus, PerformanceRecordType } from '../../constants';
import getResource from '../../resourceHelper';

const PageType = {
    MyPage: 1,
    MemberPage: 2,
    MemberProfile: 3
};

export default class PerformanceRecord extends Component {
    static propTypes = {
        data: PropTypes.object,
        pageType: PropTypes.number,
        isHighlighted: PropTypes.bool,
        onEdit: PropTypes.func
    };

    static defaultProps = {
        data: null,
        pageType: PageType.MyPage,
        isHighlighted: false,
        onEdit: function () { }
    };

    constructor(props) {
        super(props);

        this.state = {
            showCollapseExpandButton: false,
            isExpanded: false
        };
    }

    componentDidMount() {
        let record = this.props.data;
        if (record && (record.typeId === PerformanceRecordType.PerformanceCheckin || record.typeId === PerformanceRecordType.Nomination)) {
            this.detectCollapseExpandComment();
        }
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.props.data) !== JSON.stringify(nextProps.data)
            || this.props.pageType !== nextProps.pageType
            || this.props.isHighlighted !== nextProps.isHighlighted
            || this.state.showCollapseExpandButton !== nextState.showCollapseExpandButton
            || this.state.isExpanded !== nextState.isExpanded) {
            return true;
        }

        return false;
    }

    collapseExpandComment = () => {
        this.setState({
            isExpanded: !this.state.isExpanded
        });
    }

    detectCollapseExpandComment() {
        let height = this.element.offsetHeight;
        if (height > 60) {
            this.setState({
                showCollapseExpandButton: true,
                isExpanded: true
            });
        }
    }

    handleEdit = () => {
        this.props.onEdit(this.props.data.performanceRecordId);
    }

    renderPerformanceCheckin = () => {
        let record = this.props.data;
        let employees = record.employees;
        let creator = record.creator;
        let grade = record.grade ? `(${record.grade.name}) ${record.grade.description}` : 'N/A';
        let projects = record.projects && record.projects.length > 0 ?
            record.projects.map(project => <span key={project.projectId} className="tag">{project.name}</span>) : 'N/A';
        let createdDate = record.createdDate ? moment(record.createdDate).format(DateFormat) : '';
        let modifiedDate = record.modifiedDate ? moment(record.modifiedDate).format(DateFormat) : '';

        return (
            <div className={`record ${this.props.isHighlighted ? 'highlighted' : ''}`} id={`record-${record.performanceRecordId}`}>
                <div className="avatar-user">
                    <img src={creator.avatar} alt="avatar" className="avatar" />
                    <div className="person-info">
                        <span className={getRoleClassName(record.roleId)}>{creator.nickName}</span>
                        {
                            employees && employees.length > 0 && employees[0].employeeId !== creator.employeeId &&
                            <Fragment>
                                <span className="icon-checkin" />
                                <span className="role-member">{employees.map(employee => { return employee.nickName; }).join(', ')}</span>
                            </Fragment>
                        }
                        <div>
                            <span className="submit-date">{createdDate}</span>
                            {
                                record.statusId === PerformanceRecordStatus.Draft &&
                                <span className="draft">draft</span>
                            }
                        </div>
                    </div>
                    {
                        (modifiedDate || record.isHost) &&
                        <div className="date">
                            <div className="edit-date">
                                {
                                    record.statusId !== PerformanceRecordStatus.Draft && modifiedDate &&
                                    <Fragment>Edited on {modifiedDate}</Fragment>
                                }
                                {
                                    record.isHost &&
                                    <a className="icon-edit" onClick={this.handleEdit} />
                                }
                            </div>
                        </div>
                    }
                </div>
                <div className="comment" >
                    <p className={this.state.isExpanded ? "collapsed" : "expanded"} ref={(element) => this.element = element}>{record.comment}</p>
                    {
                        this.state.showCollapseExpandButton ?
                            <span className={this.state.isExpanded ? "see-more" : "see-less"} onClick={this.collapseExpandComment} />
                            : null
                    }
                </div>
                <div className="grade">
                    {`${getResource('Grade')}:`} <div className="grade-detail">{grade}</div>
                </div>
                <div className="project">
                    {`${getResource('ProjectTag')}:`} <div className="project-detail">{projects}</div>
                </div>
            </div>
        );
    }

    renderNomination = () => {
        let record = this.props.data;
        let employees = record.employees;
        let creator = record.creator;
        let createdDate = record.createdDate ? moment(record.createdDate).format(DateFormat) : '';
        let modifiedDate = record.modifiedDate ? moment(record.modifiedDate).format(DateFormat) : '';

        return (
            <div className={`record ${this.props.isHighlighted ? 'highlighted' : ''}`} id={`record-${record.performanceRecordId}`}>
                <div className="avatar-user">
                    <img src={creator.avatar} alt="avatar" className="avatar" />
                    <div className="person-info">
                        <span className={getRoleClassName(record.roleId)}>{creator.nickName}</span>
                        {
                            employees && employees.length > 0 && employees[0].employeeId !== creator.employeeId &&
                            <Fragment>
                                <span className="icon-checkin" />
                                <span className="role-member">{employees.map(employee => { return employee.nickName; }).join(', ')}</span>
                            </Fragment>
                        }
                        <div>
                            <span className="submit-date">{createdDate}</span>
                            {
                                record.statusId === PerformanceRecordStatus.Draft &&
                                <span className="draft">draft</span>
                            }
                        </div>

                    </div>
                    {
                        (modifiedDate || record.isHost) &&
                        <div className="date">
                            <div className="edit-date">
                                <div className="nomination-info">
                                    <div className="nomination-period">{this.props.nominationPeriod && this.props.nominationPeriod.title}</div>  
                                    
                                    {
                                        this.props.statusId !== PerformanceRecordStatus.Draft && this.props.modifiedDate &&
                                        <Fragment>Edited on {this.props.modifiedDate}</Fragment>
                                    }
                                </div>
                                {
                                    record.isHost &&
                                    <a className="icon-edit" onClick={this.handleEdit} />
                                }
                                
                            </div>
                        </div>
                    }
                </div>

                <div className="comment" >
                    <p className={this.state.isExpanded ? "collapsed" : "expanded"} ref={(element) => this.element = element}>{record.comment}</p>
                    {
                        this.state.showCollapseExpandButton ?
                            <span className={this.state.isExpanded ? "see-more" : "see-less"} onClick={this.collapseExpandComment} />
                            : null
                    }
                </div>
            </div>
        );
    }

    renderSystemPerformanceAppraisal = () => {
        let record = this.props.data;
        let grade = record.grade ? `(${record.grade.name}) ${record.grade.description}` : 'N/A';
        let createdDate = record.createdDate ? moment(record.createdDate).format(DateFormat) : '';

        return (
            <div className={`record record-system ${this.props.isHighlighted ? 'highlighted' : ''}`} id={`record-${record.performanceRecordId}`}>
                <div className="avatar-user">
                    <img src={resolveClientUrl('dist/avatars/nexgoal.jpg')} alt="avatar" className="avatar" />
                    <div className="person-info">
                        <span className={getRoleClassName(record.roleId)}>NexGoal - {record.period ? record.period.name : ''}</span>
                        <div>
                            <span className="submit-date">{createdDate}</span>
                        </div>
                    </div>
                </div>
                <div className="grade">
                    {`${getResource('Grade')}:`} <div className="grade-detail">{grade}</div>
                </div>
            </div>
        );
    }

    renderSystemNomination = () => {
        let record = this.props.data;
        let nominationStatusId = record.nominationStatusId;
        let nominationStatus = NominationStatuses.find(x => x.value === nominationStatusId);
        let createdDate = record.createdDate ? moment(record.createdDate).format(DateFormat) : '';
        let nominationPeriod = record.nominationPeriod ? record.nominationPeriod.title : '';
        let employees = record.employees.map(employee => { return employee.nickName; }).join(', ');
        let content = '';

        switch (nominationStatusId) {
            case NominationStatuses[0].value:
                content = this.props.pageType === PageType.MyPage ?
                    `You were in the nomination list of ${nominationPeriod}`
                    : this.props.pageType === PageType.MemberProfile ?
                        `${employees} was in the nomination list of ${nominationPeriod}`
                        : `Here is the list of nominated nominee(s): ${employees}`;
                break;

            case NominationStatuses[1].value:
                content = this.props.pageType === PageType.MyPage ?
                    `You were in the shortlist of ${nominationPeriod}`
                    : this.props.pageType === PageType.MemberProfile ?
                        `${employees} was in the shortlist of ${nominationPeriod}`
                        : `Here is the list of shortlisted nominee(s): ${employees}`;
                break;

            case NominationStatuses[2].value:
                content = this.props.pageType === PageType.MyPage ?
                    `Congratulations! You were the Winner of ${nominationPeriod}`
                    : this.props.pageType === PageType.MemberProfile ?
                        `Congratulations! ${employees} was the Winner of ${nominationPeriod}`
                        : `Congratulations to the Winner(s): ${employees}`;

                break;
        }

        return (
            <div className={`record record-system ${getNominationClassName(nominationStatusId)} ${this.props.isHighlighted ? 'highlighted' : ''}`}
                id={`record-${record.performanceRecordId}`}
            >
                {
                    this.props.pageType === PageType.MyPage &&
                    <div className="comment">{content}</div>
                }
                {
                    this.props.pageType === PageType.MemberProfile &&
                    <div className="comment">{content}</div>
                }
                {
                    this.props.pageType === PageType.MemberPage &&
                    <Fragment>
                        <div className="avatar-user">
                            <img src={resolveClientUrl('dist/avatars/nexgoal.jpg')} alt="avatar" className="avatar" />
                            <div className="person-info">
                                <span className={getRoleClassName(record.roleId)}>
                                    {record.nominationPeriod && record.nominationPeriod.title}
                                </span>
                                <div>
                                    <span className="submit-date">{createdDate}</span>
                                </div>
                            </div>
                            <div className="date">
                                <div className={getNominationClassName(nominationStatusId)}>
                                    {nominationStatus ? nominationStatus.text : ''}
                                </div>
                            </div>
                        </div>
                        <div className="comment">{content}</div>
                    </Fragment>
                }
            </div>
        );
    }

    renderRecord = () => {
        let record = this.props.data;

        switch (record.typeId) {
            case PerformanceRecordType.PerformanceCheckin:
                return this.renderPerformanceCheckin();

            case PerformanceRecordType.Nomination:
                return this.renderNomination();

            case PerformanceRecordType.SystemPerformanceAppraisal:
                return this.renderSystemPerformanceAppraisal();

            case PerformanceRecordType.SystemNomination:
                return this.renderSystemNomination();
        }
    }

    render() {
        return (
            <Fragment>
                {
                    this.props.data && this.renderRecord()
                }
            </Fragment>
        );
    }
}